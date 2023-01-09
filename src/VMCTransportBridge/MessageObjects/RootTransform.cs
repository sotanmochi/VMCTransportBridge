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
    public class RootTransform
    {
        [Key(0)]
        public string Name = "root";

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
        public float ScaleX = 1f;

        [Key(9)]
        public float ScaleY = 1f;

        [Key(10)]
        public float ScaleZ = 1f;

        [Key(11)]
        public float OffsetX = 0f;

        [Key(12)]
        public float OffsetY = 0f;

        [Key(13)]
        public float OffsetZ = 0f;

        public void SetDefault()
        {
            Name = "root";
            PositionX = 0f;
            PositionY = 0f;
            PositionZ = 0f;
            RotationX = 0f;
            RotationY = 0f;
            RotationZ = 0f;
            RotationW = 1f;
            ScaleX = 1f;
            ScaleY = 1f;
            ScaleZ = 1f;
            OffsetX = 0f;
            OffsetY = 0f;
            OffsetZ = 0f;
        }
    }
}
