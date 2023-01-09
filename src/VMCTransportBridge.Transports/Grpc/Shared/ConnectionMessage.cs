using System;
using MessagePack;

namespace VMCTransportBridge.Transports.Grpc.Shared
{
    [MessagePackObject]
    public class ConnectionMessage
    {
        [Key(0)]
        public Int64 TimestampMilliseconds;

        [Key(1)]
        public int ClientId;

        [Key(2)]
        public string ConnectionId;
    }
}