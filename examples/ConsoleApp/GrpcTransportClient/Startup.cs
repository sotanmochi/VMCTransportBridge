using System;
using System.Buffers;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using MessagePack;
using VMCTransportBridge;
using VMCTransportBridge.Serialization;
using VMCTransportBridge.Transports.Grpc.Client;
using VMCTransportBridge.Transports.Grpc.Shared;
using VMCTransportBridge.Utils;

namespace GrpcTransportClient;

public class Startup
{
    private readonly GrpcTransport _client;
    private readonly IMessageSerializer _messageSerializer;
    private readonly StringBuilder _inputMessageBuffer = new StringBuilder();

    public Startup(GrpcChannel channel, IMessageSerializer messageSerializer)
    {
        _messageSerializer = messageSerializer;
        _client = new GrpcTransport(messageSerializer, channel);
        _client.OnReceiveMessage += OnResponseEventHandler;
        Console.CancelKeyPress += ConsoleCancelEventHandler;
    }

    public void Dispose()
    {
        Console.CancelKeyPress -= ConsoleCancelEventHandler;
        _client.OnReceiveMessage -= OnResponseEventHandler;

        _client.DisposeAsync().GetAwaiter().GetResult();

        Console.WriteLine("[Startup] Disposed");
    }

    public async Task StartAsync()
    {
        var streamingMessage = new StreamingMessage();

        Console.WriteLine($"Connecting...");
        await _client.ConnectAsync();
        Console.WriteLine($"Connected: {_client.IsConnected}");

        await Task.Run(() =>
        {
        while (true)
        {
            Console.Write("Input message ('q' to quit): ");
            var message = ReadLine();

            if (message is null || message.ToLower() == "q")
            {
                break;
            }
            else if (message == string.Empty)
            {
                continue;
            }

            streamingMessage.TimestampMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            streamingMessage.TextMessage = message;

            var data = SerializeMessage(9999, _client.ClientId, streamingMessage);
            // var data = MessagePackSerializer.Serialize(streamingMessage);

            // Console.WriteLine($"-------------------------------------------");
            // Console.WriteLine($"SendAsync: {data.Length} [bytes]");
            // Console.WriteLine($"-------------------------------------------");

            _client.Send(data);
        }
        });
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
            Console.WriteLine($"[ERROR][GrpcTransportClient] ArrayLength: {arrayLength}");
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
            Console.WriteLine("");
            Console.WriteLine($"-------------------------------------------");
            Console.WriteLine($"Received data size: {serializedMessage.Length}");
            Console.WriteLine($"-------------------------------------------");
            Console.WriteLine($"Exception: {e}");
            Console.WriteLine($"-------------------------------------------");
            Console.Write($"Input message ('q' to quit): {_inputMessageBuffer}");
            return;
        }

        try
        {
            if (messageId == (int)MessageType.Connect)
            {
                var connectionMessage = _messageSerializer.Deserialize<ConnectionMessage>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));

                var timestampMilliseconds = connectionMessage.TimestampMilliseconds;
                var clientId = connectionMessage.ClientId;
                var connectionId = connectionMessage.ConnectionId;

                var localTime = DateTimeOffset.FromUnixTimeMilliseconds(connectionMessage.TimestampMilliseconds).ToLocalTime();

                Console.WriteLine("");
                Console.WriteLine($"-------------------------------------------");
                Console.WriteLine($"Received data size: {serializedMessage.Length}");
                Console.WriteLine($"-------------------------------------------");
                Console.WriteLine($"NetworkClientId: {networkClientId}");
                Console.WriteLine($"Received message: ");
                Console.WriteLine("{");
                Console.WriteLine($"  Timestamp (Milliseconds): {timestampMilliseconds} ({localTime})");
                Console.WriteLine($"  ClientId: {clientId}");
                Console.WriteLine($"  ConnectionId: {connectionId}");
                Console.WriteLine("}");
                Console.WriteLine($"-------------------------------------------");
                Console.Write($"Input message ('q' to quit): {_inputMessageBuffer}");
            }
            else
            {
                var streamingMessage = _messageSerializer.Deserialize<StreamingMessage>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));

                var timestampMilliseconds = streamingMessage.TimestampMilliseconds;
                var textMessage = streamingMessage.TextMessage;

                var localTime = DateTimeOffset.FromUnixTimeMilliseconds(streamingMessage.TimestampMilliseconds).ToLocalTime();

                Console.WriteLine("");
                Console.WriteLine($"-------------------------------------------");
                Console.WriteLine($"Received data size: {serializedMessage.Length}");
                Console.WriteLine($"-------------------------------------------");
                Console.WriteLine($"NetworkClientId: {networkClientId}");
                Console.WriteLine($"Received message: ");
                Console.WriteLine("{");
                Console.WriteLine($"  Timestamp (Milliseconds): {timestampMilliseconds} ({localTime})");
                Console.WriteLine($"  TextMessage: {textMessage}");
                Console.WriteLine("}");
                Console.WriteLine($"-------------------------------------------");
                Console.Write($"Input message ('q' to quit): {_inputMessageBuffer}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"-------------------------------------------");
            Console.WriteLine($"Received data size: {serializedMessage.Length}");
            Console.WriteLine($"-------------------------------------------");
            Console.WriteLine($"Exception: {e}");
            Console.WriteLine($"-------------------------------------------");
            Console.Write($"Input message ('q' to quit): {_inputMessageBuffer}");
        }

    }

    private void ConsoleCancelEventHandler(object? sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine();
        Console.WriteLine("Console cancel key has been pressed.");
        Dispose();
    }

    private string ReadLine()
    {
        _inputMessageBuffer.Clear();

        while (true)
        {
            var keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.Escape:
                    Console.WriteLine();
                    return string.Empty;

                case ConsoleKey.Enter:
                    Console.WriteLine();
                    return _inputMessageBuffer.ToString();

                case ConsoleKey.Backspace:
                    if(_inputMessageBuffer.Length > 0)
                    {
                        _inputMessageBuffer.Remove(_inputMessageBuffer.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                    break;

                default:
                    if(keyInfo.KeyChar != '\0')
                        _inputMessageBuffer.Append(keyInfo.KeyChar);
                    Console.Write(keyInfo.KeyChar);
                    break;
            }
        }
    }
}