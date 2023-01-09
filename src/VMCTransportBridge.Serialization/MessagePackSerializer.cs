using System.Buffers;
using MessagePack;

namespace VMCTransportBridge.Serialization
{
    public sealed class MessagePackMessageSerializer : IMessageSerializer
    {
        public static MessagePackSerializerOptions DefaultOptions { get; set; } = MessagePackSerializerOptions.Standard;

        private readonly MessagePackSerializerOptions _serializerOptions;

        public MessagePackMessageSerializer(MessagePackSerializerOptions serializerOptions = null)
        {
            _serializerOptions = (serializerOptions is null) ? DefaultOptions : serializerOptions;
        }

        public void Serialize<T>(IBufferWriter<byte> writer, in T value)
            => MessagePackSerializer.Serialize(writer, value, _serializerOptions);

        public T Deserialize<T>(in ReadOnlySequence<byte> bytes)
            => MessagePackSerializer.Deserialize<T>(bytes, _serializerOptions);
    }
}