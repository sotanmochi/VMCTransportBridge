using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Grpc.Core;
using MessagePack;
using VMCTransportBridge.Serialization;
using VMCTransportBridge.Transports.Grpc.Shared;
using VMCTransportBridge.Utils;

namespace VMCTransportBridge.Transports.Grpc.Server;

public class BinaryStreamingService : BinaryStreamingServiceBase
{
    private readonly CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly ConnectionRepository _connectionRepository;
    private readonly ClientIdPool _clientIdPool;
    private readonly IMessageSerializer _messageSerializer;
    private readonly ILogger<BinaryStreamingService> _logger;

    private ushort _clientId;

    public BinaryStreamingService
    (
        ConnectionRepository connectionRepository, 
        ClientIdPool clientIdPool,
        IMessageSerializer messageSerializer,
        ILogger<BinaryStreamingService> logger
    )
    {
        _connectionRepository = connectionRepository;
        _clientIdPool = clientIdPool;
        _messageSerializer = messageSerializer;
        _logger = logger;
    }

    public override async Task Connect
    (
        IAsyncStreamReader<byte[]> requestStream,
        IServerStreamWriter<byte[]> responseStream,
        ServerCallContext context
    )
    {
        var connectionId = Guid.NewGuid();

        try
        {
            await OnConnecting(connectionId, responseStream);

            // Main loop of streaming service.
            while (await requestStream.MoveNext(_cts.Token))
            {
                var data = requestStream.Current;
                // await _connectionRepository.BroadcastExceptAsync(data, connectionId);
                await _connectionRepository.BroadcastAsync(data);
            }
        }
        catch (Exception ex)
        {
            LogError(ex, "Exception@Connect");
        }
        finally
        {
            _connectionRepository.Remove(connectionId);
            _clientIdPool.ReturnToPool(_clientId);

            Log($"OnDisconnected - ClientId: {_clientId}, ConnectionId: {connectionId}");
            Log($"Connections: {_connectionRepository.Count}");

            _clientId = 0;
        }
    }

    private async Task OnConnecting(Guid connectionId, IServerStreamWriter<byte[]> responseStream)
    {
        if (!_clientIdPool.TryGetClientId(out _clientId))
        {
            throw new Exception("[BinaryStreamingService] Max Connection Error");
        }

        _connectionRepository.Add(connectionId, responseStream);

        Log($"OnConnected - ClientId: {_clientId}, ConnectionId: {connectionId}");
        Log($"Connections: {_connectionRepository.Count}");

        var messageId = (int)MessageType.Connect;

        var connectionMessage = new ConnectionMessage()
        {
            TimestampMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ClientId = _clientId,
            ConnectionId = connectionId.ToString()
        };

        var serializedMessage = SerializeMessage(messageId, _clientId, connectionMessage);
        await responseStream.WriteAsync(serializedMessage);
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

    private void Log(string message)
    {
        _logger.LogInformation(message);
    }

    private void LogError(Exception e, string message)
    {
        _logger.LogError(e, message);
    }
}