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
    public class RemoteVrm
    {
        [Key(0)]
        public string ServiceName = string.Empty;

        [Key(1)]
        public string Json = string.Empty;
    }
}