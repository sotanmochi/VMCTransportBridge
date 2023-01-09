using System;

namespace VMCTransportBridge
{
    public interface IMessageReceiver
    {
        event Action<PerformerAppStatus> OnReceivePerformerAppStatus;
        event Action<LocalVrm> OnReceiveLocalVrm;
        event Action<RemoteVrm> OnReceiveRemoteVrm;
        event Action<Time> OnReceiveTime;
        event Action<RootTransform> OnReceiveRootTransform;
        event Action<BoneTransform> OnReceiveBoneTransform;
        event Action<BlendShapeProxyValue> OnReceiveBlendShapeProxyValue;
        event Action<BlendShapeProxyApply> OnReceiveBlendShapeProxyApply;
        event Action<Camera> OnReceiveCamera;
        event Action<Light> OnReceiveLight;
        event Action<ControllerInput> OnReceiveControllerInput;
        event Action<KeyInput> OnReceiveKeyInput;
        event Action<DeviceTransform> OnReceiveDeviceTransform;
        event Action<DeviceLocalTransform> OnReceiveDeviceLocalTransform;

        //////////////////////////////////////////////////
        ///  Extensions for network transport bridge   ///
        //////////////////////////////////////////////////
        event Action<PerformerAppStatus, int> OnReceiveTransportedPerformerAppStatus;
        event Action<LocalVrm, int> OnReceiveTransportedLocalVrm;
        event Action<RemoteVrm, int> OnReceiveTransportedRemoteVrm;
        event Action<Time, int> OnReceiveTransportedTime;
        event Action<RootTransform, int> OnReceiveTransportedRootTransform;
        event Action<BoneTransform, int> OnReceiveTransportedBoneTransform;
        event Action<BlendShapeProxyValue, int> OnReceiveTransportedBlendShapeProxyValue;
        event Action<BlendShapeProxyApply, int> OnReceiveTransportedBlendShapeProxyApply;
        event Action<Camera, int> OnReceiveTransportedCamera;
        event Action<Light, int> OnReceiveTransportedLight;
        event Action<ControllerInput, int> OnReceiveTransportedControllerInput;
        event Action<KeyInput, int> OnReceiveTransportedKeyInput;
        event Action<DeviceTransform, int> OnReceiveTransportedDeviceTransform;
        event Action<DeviceLocalTransform, int> OnReceiveTransportedDeviceLocalTransform;
        //////////////////////////////////////////////////

        bool IsRunning { get; }
        ushort Port { get; }

        void Start(ushort port);
        void Stop();
    }
}