using System;
using System.Collections.Concurrent;

namespace VMCTransportBridge.Transports.Grpc.Server
{
    public class ClientIdPool
    {
        public static readonly ushort DefaultCapacity = 1024;
        private readonly ConcurrentQueue<ushort> _clientIdPool = new ConcurrentQueue<ushort>();

        public ClientIdPool(ushort capacity = 0)
        {
            if (capacity < 1)
            {
                capacity = DefaultCapacity;
            }

            for (ushort i = 1; i < capacity; i++)
            {
                _clientIdPool.Enqueue(i);
            }
        }

        public bool TryGetClientId(out ushort clientId)
        {
            return _clientIdPool.TryDequeue(out clientId);
        }

        public void ReturnToPool(ushort clientId)
        {
            _clientIdPool.Enqueue(clientId);
        }
    }
}