#if ENABLE_MONO || ENABLE_IL2CPP
#define UNITY_ENGINE
#endif

using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MessagePack;
using VMCTransportBridge.Serialization;
using VMCTransportBridge.Transports.Grpc.Shared;

namespace VMCTransportBridge.Transports.Grpc.Client
{

    public sealed class GrpcTransport : ITransport
    {
        public event Action<byte[]> OnReceiveMessage;
        public event Action OnConnected;

        public bool IsConnected => _connected;
        public int ClientId => _networkClientId;

        public string ServerAddress => _channel.Target;

        private readonly IMessageSerializer _messageSerializer;
        private readonly ChannelBase _channel;
        private readonly string _host;
        private readonly CallOptions _options;
        private readonly CallInvoker _callInvoker;

        private bool _connected;
        private int _networkClientId = -1;
        private string _connectionId = "";

        private AsyncDuplexStreamingCall<byte[], byte[]> _streamingCall;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private TaskCompletionSource<bool> _onConnected = new TaskCompletionSource<bool>();

        public GrpcTransport(IMessageSerializer messageSerializer, ChannelBase channel, string host = null, CallOptions options = default(CallOptions))
        {
            _messageSerializer = messageSerializer;
            _channel = channel;
            _host = host;
            _options = options;
            _callInvoker = channel.CreateCallInvoker();
        }

        public async Task DisposeAsync()
        {
            // Log("Disposing");
            
            try
            {
                if (_streamingCall != null)
                {
                    await _streamingCall.RequestStream.CompleteAsync().ConfigureAwait(false);
                    // Log("RequestStream.CompleteAsync");
                }
            }
            finally
            {
                _cts.Cancel();
                _cts.Dispose();
                // Log("Finally");
            }
            
            // Log("Disposed");
        }

        public async Task<bool> ConnectAsync(string roomId = "")
        {
            _cts = new CancellationTokenSource();
            _onConnected = new TaskCompletionSource<bool>();

            ConnectAndForget();
            _connected = await _onConnected.Task;

            return _connected;
        }

        public async Task DisconnectAsync()
        {
            DisposeAsync();
            _connected = false;
        }

        public void Send(ArraySegment<byte> serializedMessage)
        {
            _streamingCall.RequestStream.WriteAsync(serializedMessage.Array).GetAwaiter().GetResult();
        }

        public async void ConnectAndForget()
        {
            try
            {
                await ConnectAndSubscribeAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                const string message = "An error occurred while connecting to the server.";
                LogError($"{message}\n{e}");
            }
        }

        private async Task ConnectAndSubscribeAsync()
        {
            var syncContext = SynchronizationContext.Current; // Capture SynchronizationContext.
            
            try
            {
                using (_streamingCall = _callInvoker.AsyncDuplexStreamingCall<byte[], byte[]>(BinaryStreamingGrpc.ConnectMethod, _host, _options))
                {
                    var streamReader = _streamingCall.ResponseStream;
                    while (await streamReader.MoveNext(_cts.Token).ConfigureAwait(false))
                    {
                        try
                        {
                            ConsumeData(syncContext, streamReader.Current);
                        }
                        catch (Exception e)
                        {
                            const string message = "An error occurred when consuming a received message, but the subscription is still alive.";
                            LogError($"{message}\n{e}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _streamingCall = null;
                const string message = "An error occurred while subscribing to messages.";
                LogError($"{message}\n{e}");
            }
            finally
            {
                Log("OnDisconnected");
                await DisposeAsync().ConfigureAwait(false);
            }
        }

        private void ConsumeData(SynchronizationContext syncContext, byte[] data)
        {
            if (!_connected)
            {
                var messagePackReader = new MessagePackReader(data);

                var arrayLength = messagePackReader.ReadArrayHeader();
                if (arrayLength != 3)
                {
                    throw new InvalidOperationException($"[GrpcTransport] ArrayLength: {arrayLength}");
                }

                var messageId = messagePackReader.ReadInt32();
                var networkClientId = messagePackReader.ReadInt32();
                var offset = (int)messagePackReader.Consumed;

                if (messageId == (int)MessageType.Connect)
                {
                    var connectionMessage = _messageSerializer.Deserialize<ConnectionMessage>(new ReadOnlySequence<byte>(data, offset, data.Length - offset));

                    var TimestampMilliseconds = connectionMessage.TimestampMilliseconds;
                    var clientId = connectionMessage.ClientId;
                    var connectionId = connectionMessage.ConnectionId;

                    var localTime = DateTimeOffset.FromUnixTimeMilliseconds(connectionMessage.TimestampMilliseconds).ToLocalTime();

                    _networkClientId = clientId;
                    _connectionId = connectionId;

                    OnConnected?.Invoke();
                    _onConnected?.TrySetResult(true);
                }
            }
            else
            {
                if (syncContext != null)
                {
                    syncContext.Post(_ => OnReceiveMessage?.Invoke(data), null);
                }
                else
                {
                    OnReceiveMessage?.Invoke(data);
                }
            }
        }

        [
            System.Diagnostics.Conditional("DEVELOPMENT_BUILD"), 
            System.Diagnostics.Conditional("UNITY_EDITOR"),
        ]
        private void Log(object message)
        {
    #if UNITY_ENGINE
            UnityEngine.Debug.Log($"[BinaryStreamingClient] {message}");
    #else
            Console.WriteLine($"[BinaryStreamingClient] {message}");
    #endif
        }

        private void LogError(object message)
        {
    #if UNITY_ENGINE
            UnityEngine.Debug.LogError($"[BinaryStreamingClient] {message}");
    #else
            Console.WriteLine($"[ERROR][BinaryStreamingClient] {message}");
    #endif
        }
    }
}