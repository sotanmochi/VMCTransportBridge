// This code is based on the VMC Protocol specification.
//
// References:
//   - https://protocol.vmc.info/english
//   - https://protocol.vmc.info/marionette-spec
//   - https://protocol.vmc.info/performer-spec
//
namespace VMCTransportBridge
{
    public static class OscAddress
    {
        //////////////////////////////////////////////////////////////////
        /// Message types for Marionette (Motion receiver application) ///
        ///                                                            ///
        /// Performer -> Marionette                                    ///
        //////////////////////////////////////////////////////////////////
        public const string PerformerAppStatus = "/VMC/Ext/OK";
        public const string PerformerAppReceivingStatus = "/VMC/Ext/Rcv";
        public const string LocalVrm = "/VMC/Ext/VRM";
        public const string RemoteVrm = "/VMC/Ext/Remote";
        public const string Time = "/VMC/Ext/T";
        public const string RootTransform = "/VMC/Ext/Root/Pos";
        public const string BoneTransform = "/VMC/Ext/Bone/Pos";
        public const string ControllerInput = "/VMC/Ext/Con";
        public const string KeyInput = "/VMC/Ext/Key";
        public const string MidiNoteInput = "/VMC/Ext/Midi/Note";
        public const string MidiCcButtonInput = "/VMC/Ext/Midi/CC/Bit";
        public const string OptionString = "/VMC/Ext/Opt";
        public const string BackgroundColor = "/VMC/Ext/Setting/Color";
        public const string WindowAttribute = "/VMC/Ext/Setting/Win";
        public const string Config = "/VMC/Ext/Config";

        /////////////////////////////////////////////////////////
        /// Common message types for                          ///
        /// Marionette (Motion receiver application) and      ///
        /// Performer (Motion sender application).            ///
        ///                                                   ///
        /// Performer  -> Marionette or                       ///
        /// Marionette -> Performer  or                       ///
        /// Assistant  -> Performer                           ///
        /////////////////////////////////////////////////////////
        public const string BlendShapeProxyValue = "/VMC/Ext/Blend/Val";
        public const string BlendShapeProxyApply = "/VMC/Ext/Blend/Apply";
        public const string Camera = "/VMC/Ext/Cam";
        public const string Light = "/VMC/Ext/Light";
        public const string MidiCcValInput = "/VMC/Ext/Midi/CC/Val";
        public const string HmdDeviceTransform = "/VMC/Ext/Hmd/Pos";
        public const string ControllerDeviceTransform = "/VMC/Ext/Con/Pos";
        public const string TrackerDeviceTransform = "/VMC/Ext/Tra/Pos";
        public const string HmdDeviceLocalTransform = "/VMC/Ext/Hmd/Pos/Local";
        public const string ControllerDeviceLocalTransform = "/VMC/Ext/Con/Pos/Local";
        public const string TrackerDeviceLocalTransform = "/VMC/Ext/Tra/Pos/Local";

        ///////////////////////////////////////////////////////////////
        /// Message types for Performer (Motion sender application) ///
        ///                                                         ///
        /// Marionette -> Performer or                              ///
        /// Assistant  -> Performer                                 ///
        ///////////////////////////////////////////////////////////////
        public const string FramePeriod = "/VMC/Ext/Set/Period";
        public const string EyeTrackingTargetPosition = "/VMC/Ext/Set/Eye";
        public const string RequestInformation = "/VMC/Ext/Set/Req";
        public const string ResponseString = "/VMC/Ext/Set/Res";
        public const string CalibrationReady = "/VMC/Ext/Set/Calib/Ready";
        public const string CalibrationExec = "/VMC/Ext/Set/Calib/Exec";
        public const string SetConfig = "/VMC/Ext/Set/Config";

        ///////////////////////////////////////////////
        /// Extensions for network transport bridge ///
        ///////////////////////////////////////////////
        public const string TransportedPerformerAppStatus = "/VMC/Ext/OK/Transported";
        public const string TransportedLocalVrm = "/VMC/Ext/VRM/Transported";
        public const string TransportedRemoteVrm = "/VMC/Ext/Remote/Transported";
        public const string TransportedTime = "/VMC/Ext/T/Transported";
        public const string TransportedRootTransform = "/VMC/Ext/Root/Pos/Transported";
        public const string TransportedBoneTransform = "/VMC/Ext/Bone/Pos/Transported";
        public const string TransportedBlendShapeProxyValue = "/VMC/Ext/Blend/Val/Transported";
        public const string TransportedBlendShapeProxyApply = "/VMC/Ext/Blend/Apply/Transported";
        public const string TransportedCamera = "/VMC/Ext/Cam/Transported";
        public const string TransportedLight = "/VMC/Ext/Light/Transported";
        public const string TransportedControllerInput = "/VMC/Ext/Con/Transported";
        public const string TransportedKeyInput = "/VMC/Ext/Key/Transported";
        // public const string TransportedMidiNoteInput = "/VMC/Ext/Midi/Note/Transported";
        // public const string TransportedMidiCCValInput = "/VMC/Ext/Midi/CC/Val/Transported";
        // public const string TransportedMidiCCButtonInput = "/VMC/Ext/Midi/CC/Bit/Transported";
        public const string TransportedHmdDeviceTransform = "/VMC/Ext/Hmd/Pos/Transported";
        public const string TransportedControllerDeviceTransform = "/VMC/Ext/Con/Pos/Transported";
        public const string TransportedTrackerDeviceTransform = "/VMC/Ext/Tra/Pos/Transported";
        public const string TransportedHmdDeviceLocalTransform = "/VMC/Ext/Hmd/Pos/Local/Transported";
        public const string TransportedControllerDeviceLocalTransform = "/VMC/Ext/Con/Pos/Local/Transported";
        public const string TransportedTrackerDeviceLocalTransform = "/VMC/Ext/Tra/Pos/Local/Transported";
    }
}
