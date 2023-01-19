using System;
using System.Threading;
using System.Threading.Tasks;

namespace VMCTransportBridge
{
    public interface ITransport
    {
        event Action<byte[]> OnReceiveMessage;

        bool IsConnected { get; }
        int ClientId { get; }

        void Send(ArraySegment<byte> data);

        Task<bool> ConnectAsync(string roomId = "");
        Task DisconnectAsync();
        Task DisposeAsync();
    }
}