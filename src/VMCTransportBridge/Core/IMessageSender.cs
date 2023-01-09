using System;

namespace VMCTransportBridge
{
    public interface IMessageSender
    {
        event Action<PerformerAppStatus> OnSendPerformerAppStatus;
        event Action<LocalVrm> OnSendLocalVrm;
        event Action<RemoteVrm> OnSendRemoteVrm;
        event Action<Time> OnSendTime;
        event Action<RootTransform> OnSendRootTransform;
        event Action<BoneTransform> OnSendBoneTransform;
        event Action<BlendShapeProxyValue> OnSendBlendShapeProxyValue;
        event Action<BlendShapeProxyApply> OnSendBlendShapeProxyApply;
        event Action<Camera> OnSendCamera;
        event Action<Light> OnSendLight;
        event Action<ControllerInput> OnSendControllerInput;
        event Action<KeyInput> OnSendKeyInput;
        event Action<DeviceTransform> OnSendDeviceTransform;
        event Action<DeviceLocalTransform> OnSendDeviceLocalTransform;

        //////////////////////////////////////////////////
        ///  Extensions for network transport bridge   ///
        //////////////////////////////////////////////////
        event Action<PerformerAppStatus, int> OnSendTransportedPerformerAppStatus;
        event Action<LocalVrm, int> OnSendTransportedLocalVrm;
        event Action<RemoteVrm, int> OnSendTransportedRemoteVrm;
        event Action<Time, int> OnSendTransportedTime;
        event Action<RootTransform, int> OnSendTransportedRootTransform;
        event Action<BoneTransform, int> OnSendTransportedBoneTransform;
        event Action<BlendShapeProxyValue, int> OnSendTransportedBlendShapeProxyValue;
        event Action<BlendShapeProxyApply, int> OnSendTransportedBlendShapeProxyApply;
        event Action<Camera, int> OnSendTransportedCamera;
        event Action<Light, int> OnSendTransportedLight;
        event Action<ControllerInput, int> OnSendTransportedControllerInput;
        event Action<KeyInput, int> OnSendTransportedKeyInput;
        event Action<DeviceTransform, int> OnSendTransportedDeviceTransform;
        event Action<DeviceLocalTransform, int> OnSendTransportedDeviceLocalTransform;
        //////////////////////////////////////////////////

        bool IsRunning { get; }
        string DestinationAddress { get; }
        ushort DestinationPort { get; }

        void Start(string destinationAddress = "127.0.0.1", ushort destinationPort = 39539);
        void Stop();

        void SendPerformerAppStatus(PerformerAppStatus status);
        void SendTransportedPerformerAppStatus(PerformerAppStatus status, int networkClientId = -1);
        void SendLocalVrm(LocalVrm data);
        void SendTransportedLocalVrm(LocalVrm data, int networkClientId = -1);
        void SendRemoteVrm(RemoteVrm data);
        void SendTransportedRemoteVrm(RemoteVrm data, int networkClientId = -1);
        void SendTime(float time);
        void SendTransportedTime(float time, int networkClientId = -1);
        void SendRootTransform(RootTransform rootTransform);
        void SendTransportedRootTransform(RootTransform rootTransform, int networkClientId = -1);
        void SendBoneTransform(BoneTransform boneTransform);
        void SendTransportedBoneTransform(BoneTransform boneTransform, int networkClientId = -1);
        void SendBlendShapeProxyValue(BlendShapeProxyValue data);
        void SendTransportedBlendShapeProxyValue(BlendShapeProxyValue data, int networkClientId = -1);
        void SendBlendShapeProxyApply();
        void SendTransportedBlendShapeProxyApply(int networkClientId = -1);
        void SendCamera(Camera camera);
        void SendTransportedCamera(Camera camera, int networkClientId = -1);
        void SendLight(Light light);
        void SendTransportedLight(Light light, int networkClientId = -1);
        void SendControllerInput(ControllerInput controllerInput);
        void SendTransportedControllerInput(ControllerInput controllerInput, int networkClientId = -1);
        void SendKeyInput(KeyInput keyInput);
        void SendTransportedKeyInput(KeyInput keyInput, int networkClientId = -1);
        void SendDeviceTransform(DeviceTransform deviceTransform);
        void SendTransportedDeviceTransform(DeviceTransform deviceTransform, int networkClientId = -1);
        void SendDeviceLocalTransform(DeviceLocalTransform deviceTransform);
        void SendTransportedDeviceLocalTransform(DeviceLocalTransform deviceTransform, int networkClientId = -1);
    }
}