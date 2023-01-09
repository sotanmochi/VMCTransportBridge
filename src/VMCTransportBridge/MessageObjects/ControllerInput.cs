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
    public class ControllerInput
    {
        [Key(0)]
        public int Active;

        [Key(1)]
        public string Name = "Unknown";

        [Key(2)]
        public int IsLeft;

        [Key(3)]
        public int IsTouch;

        [Key(4)]
        public int IsAxis;

        [Key(5)]
        public float AxisX;

        [Key(6)]
        public float AxisY;

        [Key(7)]
        public float AxisZ;
    }
}
