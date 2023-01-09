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
    public class PerformerAppStatus
    {
        /// <summary>
        /// Not loaded = 0, 
        /// Loaded = 1
        /// </summary>
        [Key(0)]
        public int Loaded = 0;

        /// <summary>
        /// Uncalibrated = 0,
        /// WaitingForCalibrating = 1, 
        /// Calibrating = 2, 
        /// Calibrated = 3
        /// </summary>
        [Key(1)]
        public int CalibrationState = 3;

        /// <summary>
        /// Normal = 0, 
        /// MR Normal = 1, 
        /// MR Floor fix = 2
        /// </summary>
        [Key(2)]
        public int CalibrationMode = 0;

        /// <summary>
        /// Bad = 0, 
        /// OK = 1
        /// </summary>
        [Key(3)]
        public int TrackingStatus = 1;

        public void SetDefault()
        {
            Loaded = 0;
            CalibrationState = 3;
            CalibrationMode = 0;
            TrackingStatus = 1;
        }
    }
}
