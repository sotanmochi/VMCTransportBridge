namespace VMCTransportBridge
{
    public enum MessageType
    {
        // Transport Messages
        Connect              = 1,

        // VMC Protocol Messages
        PerformerAppStatus   = 10,
        LocalVrm             = 11,
        RemoteVrm            = 12,
        Time                 = 13,
        RootTransform        = 14,
        BoneTransform        = 15,
        BlendShapeProxyValue = 16,
        BlendShapeProxyApply = 17,
        Camera               = 18,
        Light                = 19,
        ControllerInput      = 20,
        KeyInput             = 21,
        DeviceTransform      = 22,
        DeviceLocalTransform = 23,
    }
}