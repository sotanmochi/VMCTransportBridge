using System;
using System.Buffers;
using System.Threading.Tasks;
using Grpc.Core;
using MessagePack;
using UnityEngine;
using VMCTransportBridge;
using VMCTransportBridge.Serialization;
using VMCTransportBridge.Transports.Grpc.Client;
using VMCTransportBridge.Utils;

namespace TransportClient.Unity
{
    public class TransportClient : IDisposable
    {
        private readonly ITransport _transport;
        private readonly IMessageSerializer _messageSerializer;
        
        public TransportClient(IMessageSerializer messageSerializer, string serverAddress)
        {
            var uri = new Uri(serverAddress);
            var target = uri.Authority;
            var credentials = (uri.Scheme == Uri.UriSchemeHttps) ? ChannelCredentials.SecureSsl : ChannelCredentials.Insecure;

            _messageSerializer = messageSerializer;

            _transport = new GrpcTransport(messageSerializer, new Channel(target, credentials));
            _transport.OnReceiveMessage += OnResponseEventHandler;
        }
        
        public void Dispose()
        {
            _transport.OnReceiveMessage -= OnResponseEventHandler;
            _transport.DisposeAsync().GetAwaiter().GetResult();
        }
        
        public async Task<bool> ConnectAsync()
        {
            return await _transport.ConnectAsync();
        }
        
        public async Task DisconnectAsync()
        {
            await _transport.DisconnectAsync();
        }
        
        public void SendMessage(string message)
        {
            var streamingMessage = new StreamingMessage();
            
            streamingMessage.TimestampMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            streamingMessage.TextMessage = message;
            
            var serializedMessage = SerializeMessage(100, _transport.ClientId, streamingMessage);
            _transport.Send(serializedMessage);
        }
        
        private byte[] SerializeMessage<T>(int messageId, int transportClientId, T message)
        {
            using (var buffer = ArrayPoolBufferWriter.RentThreadStaticWriter())
            {
                var writer = new MessagePackWriter(buffer);
                writer.WriteArrayHeader(3);
                writer.Write(messageId);
                writer.Write(transportClientId);
                writer.Flush();
                _messageSerializer.Serialize(buffer, message);
                return buffer.WrittenSpan.ToArray();
            }
        }
        
        private void OnResponseEventHandler(byte[] serializedMessage)
        {
            var messagePackReader = new MessagePackReader(serializedMessage);
            
            var arrayLength = messagePackReader.ReadArrayHeader();
            if (arrayLength != 3)
            {
                Debug.LogError($"[GrpcTransportClient] ArrayLength: {arrayLength}");
            }
            
            var messageId = -1;
            var networkClientId = -1;
            var offset = 0;
            
            try
            {
                messageId = messagePackReader.ReadInt32();
                networkClientId = messagePackReader.ReadInt32();
                offset = (int)messagePackReader.Consumed;
            }
            catch (Exception e)
            {
                Debug.Log($"-------------------------------------------");
                Debug.Log($"Received data size: {serializedMessage.Length}");
                Debug.LogError($"Exception: {e}");
                Debug.Log($"-------------------------------------------");
                return;
            }
            
            try
            {
                var streamingMessage = _messageSerializer.Deserialize<StreamingMessage>(
                    new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                
                var timestampMilliseconds = streamingMessage.TimestampMilliseconds;
                var textMessage = streamingMessage.TextMessage;
                
                var localTime = DateTimeOffset.FromUnixTimeMilliseconds(streamingMessage.TimestampMilliseconds).ToLocalTime();
                
                Debug.Log($"-------------------------------------------");
                Debug.Log($"Received data size: {serializedMessage.Length}");
                Debug.Log($"NetworkClientId: {networkClientId}");
                Debug.Log($"Timestamp (Milliseconds): {timestampMilliseconds} ({localTime})");
                Debug.Log($"TextMessage: {textMessage}");
                Debug.Log($"-------------------------------------------");
            }
            catch (Exception e)
            {
                Debug.Log($"[GrpcTransportClient] ArrayLength: {arrayLength}");
                Debug.Log($"-------------------------------------------");
                Debug.Log($"Received data size: {serializedMessage.Length}");
                Debug.Log($"-------------------------------------------");
                Debug.LogError($"Exception: {e}");
            }
        }
    }
}