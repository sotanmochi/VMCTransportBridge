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
    public class Light
    {
        [Key(0)]
        public string Name = "Unknown";

        [Key(1)]
        public float PositionX = 0f;

        [Key(2)]
        public float PositionY = 0f;

        [Key(3)]
        public float PositionZ = 0f;

        [Key(4)]
        public float RotationX = 0f;

        [Key(5)]
        public float RotationY = 0f;

        [Key(6)]
        public float RotationZ = 0f;

        [Key(7)]
        public float RotationW = 1f;

        [Key(8)]
        public float ColorRed = 1f;

        [Key(9)]
        public float ColorGreen = 1f;

        [Key(10)]
        public float ColorBlue = 1f;

        [Key(11)]
        public float ColorAlpha = 1f;
    }
}
