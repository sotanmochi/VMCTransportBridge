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
    public enum DeviceType
    {
        Unknown,
        HeadMountedDisplay,
        Controller,
        Tracker,
    }

    [MessagePackObject]
    public class DeviceTransform
    {
        [Key(0)]
        public DeviceType DeviceType;

        [Key(1)]
        public string Serial = "Unknown";

        [Key(2)]
        public float PositionX = 0f;

        [Key(3)]
        public float PositionY = 0f;

        [Key(4)]
        public float PositionZ = 0f;

        [Key(5)]
        public float RotationX = 0f;

        [Key(6)]
        public float RotationY = 0f;

        [Key(7)]
        public float RotationZ = 0f;

        [Key(8)]
        public float RotationW = 1f;
    }

    [MessagePackObject]
    public class DeviceLocalTransform
    {
        [Key(0)]
        public DeviceType DeviceType;

        [Key(1)]
        public string Serial = "Unknown";

        [Key(2)]
        public float PositionX = 0f;

        [Key(3)]
        public float PositionY = 0f;

        [Key(4)]
        public float PositionZ = 0f;

        [Key(5)]
        public float RotationX = 0f;

        [Key(6)]
        public float RotationY = 0f;

        [Key(7)]
        public float RotationZ = 0f;

        [Key(8)]
        public float RotationW = 1f;
    }
}
