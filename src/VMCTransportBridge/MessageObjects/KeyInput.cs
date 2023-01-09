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
    public class KeyInput
    {
        [Key(0)]
        public int Active;

        [Key(1)]
        public string Name = "Unknown";

        [Key(2)]
        public int Keycode;
    }
}