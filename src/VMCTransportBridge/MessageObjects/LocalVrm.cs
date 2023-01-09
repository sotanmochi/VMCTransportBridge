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
    public class LocalVrm
    {
        [Key(0)]
        public string Path = string.Empty;

        [Key(1)]
        public string Title = string.Empty;

        [Key(2)]
        public string Hash = string.Empty;
    }
}