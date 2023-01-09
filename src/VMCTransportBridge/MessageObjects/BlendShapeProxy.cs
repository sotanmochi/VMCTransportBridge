// This code is based on the VMC Protocol specification.
//
// References:
//   - https://protocol.vmc.info/english
//   - https://protocol.vmc.info/marionette-spec
//   - https://protocol.vmc.info/performer-spec
//
using MessagePack;

namespace VMCTransportBridge
{
    [MessagePackObject]
    public class BlendShapeProxyValue
    {
        [Key(0)]
        public string Name = "Unknown";

        [Key(1)]
        public float Value;
    }

    [MessagePackObject]
    public struct BlendShapeProxyApply
    {
        [Key(0)]
        public bool Value;
    }
}