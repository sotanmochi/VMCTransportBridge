using System.Buffers;

namespace VMCTransportBridge.Serialization
{
    public interface IMessageSerializer
    {
        void Serialize<T>(IBufferWriter<byte> writer, in T value);
        T Deserialize<T>(in ReadOnlySequence<byte> bytes);
    }
}