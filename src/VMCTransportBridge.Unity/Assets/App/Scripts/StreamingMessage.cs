using System;
using MessagePack;

namespace TransportClient.Unity
{
    [MessagePackObject]
    public class StreamingMessage
    {
        [Key(0)]
        public Int64 TimestampMilliseconds;

        [Key(1)]
        public string TextMessage;
    }
}