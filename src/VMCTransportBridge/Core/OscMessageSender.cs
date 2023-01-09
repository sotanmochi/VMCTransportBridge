// This code is based on the VMC Protocol specification.
//
// References:
//   - https://protocol.vmc.info/english
//   - https://protocol.vmc.info/marionette-spec
//   - https://protocol.vmc.info/performer-spec
//
using System;
using OscJack;
using OscJack.Extensions;

namespace VMCTransportBridge
{
    public sealed class OscMessageSender : IMessageSender
    {
        public event Action<PerformerAppStatus> OnSendPerformerAppStatus;
        public event Action<LocalVrm> OnSendLocalVrm;
        public event Action<RemoteVrm> OnSendRemoteVrm;
        public event Action<Time> OnSendTime;
        public event Action<RootTransform> OnSendRootTransform;
        public event Action<BoneTransform> OnSendBoneTransform;
        public event Action<BlendShapeProxyValue> OnSendBlendShapeProxyValue;
        public event Action<BlendShapeProxyApply> OnSendBlendShapeProxyApply;
        public event Action<Camera> OnSendCamera;
        public event Action<Light> OnSendLight;
        public event Action<ControllerInput> OnSendControllerInput;
        public event Action<KeyInput> OnSendKeyInput;
        public event Action<DeviceTransform> OnSendDeviceTransform;
        public event Action<DeviceLocalTransform> OnSendDeviceLocalTransform;

        //////////////////////////////////////////////////
        ///  Extensions for network transport bridge   ///
        //////////////////////////////////////////////////
        public event Action<PerformerAppStatus, int> OnSendTransportedPerformerAppStatus;
        public event Action<LocalVrm, int> OnSendTransportedLocalVrm;
        public event Action<RemoteVrm, int> OnSendTransportedRemoteVrm;
        public event Action<Time, int> OnSendTransportedTime;
        public event Action<RootTransform, int> OnSendTransportedRootTransform;
        public event Action<BoneTransform, int> OnSendTransportedBoneTransform;
        public event Action<BlendShapeProxyValue, int> OnSendTransportedBlendShapeProxyValue;
        public event Action<BlendShapeProxyApply, int> OnSendTransportedBlendShapeProxyApply;
        public event Action<Camera, int> OnSendTransportedCamera;
        public event Action<Light, int> OnSendTransportedLight;
        public event Action<ControllerInput, int> OnSendTransportedControllerInput;
        public event Action<KeyInput, int> OnSendTransportedKeyInput;
        public event Action<DeviceTransform, int> OnSendTransportedDeviceTransform;
        public event Action<DeviceLocalTransform, int> OnSendTransportedDeviceLocalTransform;
        //////////////////////////////////////////////////

        public bool IsRunning => _isRunning;
        public string DestinationAddress => _address;
        public ushort DestinationPort => _port;

        private OscClient _oscClient;
        private bool _isRunning;
        private string _address;
        private ushort _port;

        public OscMessageSender(string defaultAddress = "127.0.0.1", ushort defaultPort = 39539)
        {
            _address = defaultAddress;
            _port = defaultPort;
        }

        public void Start(string destinationAddress = "127.0.0.1", ushort destinationPort = 39539)
        {
            _address = destinationAddress;
            _port = destinationPort;
            _oscClient = new OscClient(destinationAddress, destinationPort);
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
            _oscClient?.Dispose();
            _oscClient = null;
        }

        public void SendPerformerAppStatus(PerformerAppStatus status)
        {
            if (IsRunning)
            {
                OnSendPerformerAppStatus?.Invoke(status);
                _oscClient.Send(OscAddress.PerformerAppStatus, status.Loaded, status.CalibrationState, status.CalibrationMode, status.TrackingStatus);
            }
        }

        public void SendTransportedPerformerAppStatus(PerformerAppStatus status, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedPerformerAppStatus?.Invoke(status, networkClientId);
                _oscClient.Send(OscAddress.TransportedPerformerAppStatus, status.Loaded, status.CalibrationState, status.CalibrationMode, status.TrackingStatus, networkClientId);
            }
        }

        public void SendLocalVrm(LocalVrm data)
        {
            if (IsRunning)
            {
                OnSendLocalVrm?.Invoke(data);
                _oscClient.Send(OscAddress.LocalVrm, data.Path, data.Title, data.Hash);
            }
        }

        public void SendTransportedLocalVrm(LocalVrm data, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedLocalVrm?.Invoke(data, networkClientId);
                _oscClient.Send(OscAddress.TransportedLocalVrm, data.Path, data.Title, data.Hash, networkClientId);
            }
        }

        public void SendRemoteVrm(RemoteVrm data)
        {
            if (IsRunning)
            {
                OnSendRemoteVrm?.Invoke(data);
                _oscClient.Send(OscAddress.RemoteVrm, data.ServiceName, data.Json);
            }
        }

        public void SendTransportedRemoteVrm(RemoteVrm data, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedRemoteVrm?.Invoke(data, networkClientId);
                _oscClient.Send(OscAddress.TransportedRemoteVrm, data.ServiceName, data.Json, networkClientId);
            }
        }

        public void SendTime(float time)
        {
            if (IsRunning)
            {
                OnSendTime?.Invoke(new Time(){ Value = time });
                _oscClient.Send(OscAddress.Time, time);
            }
        }

        public void SendTransportedTime(float time, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedTime?.Invoke(new Time(){ Value = time }, networkClientId);
                _oscClient.Send(OscAddress.TransportedTime, time, networkClientId);
            }
        }

        public void SendRootTransform(RootTransform rootTransform)
        {
            if (IsRunning)
            {
                OnSendRootTransform?.Invoke(rootTransform);
                _oscClient.Send(OscAddress.RootTransform, rootTransform.Name, 
                                rootTransform.PositionX, rootTransform.PositionY, rootTransform.PositionZ, 
                                rootTransform.RotationX, rootTransform.RotationY, rootTransform.RotationZ, rootTransform.RotationW, 
                                rootTransform.ScaleX, rootTransform.ScaleY, rootTransform.ScaleZ, rootTransform.OffsetX, rootTransform.OffsetY, rootTransform.OffsetZ);
            }
        }

        public void SendTransportedRootTransform(RootTransform rootTransform, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedRootTransform?.Invoke(rootTransform, networkClientId);
                _oscClient.Send(OscAddress.TransportedRootTransform, rootTransform.Name, 
                                rootTransform.PositionX, rootTransform.PositionY, rootTransform.PositionZ, 
                                rootTransform.RotationX, rootTransform.RotationY, rootTransform.RotationZ, rootTransform.RotationW, 
                                rootTransform.ScaleX, rootTransform.ScaleY, rootTransform.ScaleZ, rootTransform.OffsetX, rootTransform.OffsetY, rootTransform.OffsetZ, networkClientId);
            }
        }

        public void SendBoneTransform(BoneTransform boneTransform)
        {
            if (IsRunning)
            {
                OnSendBoneTransform?.Invoke(boneTransform);
                _oscClient.Send(OscAddress.BoneTransform, boneTransform.Name, 
                                boneTransform.PositionX, boneTransform.PositionY, boneTransform.PositionZ, 
                                boneTransform.RotationX, boneTransform.RotationY, boneTransform.RotationZ, boneTransform.RotationW);
            }
        }

        public void SendTransportedBoneTransform(BoneTransform boneTransform, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedBoneTransform?.Invoke(boneTransform, networkClientId);
                _oscClient.Send(OscAddress.TransportedBoneTransform, boneTransform.Name, 
                                boneTransform.PositionX, boneTransform.PositionY, boneTransform.PositionZ, 
                                boneTransform.RotationX, boneTransform.RotationY, boneTransform.RotationZ, boneTransform.RotationW, networkClientId);
            }
        }

        public void SendBlendShapeProxyValue(BlendShapeProxyValue data)
        {
            if (IsRunning)
            {
                OnSendBlendShapeProxyValue?.Invoke(data);
                _oscClient.Send(OscAddress.BlendShapeProxyValue, data.Name, data.Value);
            }
        }

        public void SendTransportedBlendShapeProxyValue(BlendShapeProxyValue data, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedBlendShapeProxyValue?.Invoke(data, networkClientId);
                _oscClient.Send(OscAddress.TransportedBlendShapeProxyValue, data.Name, data.Value, networkClientId);
            }
        }

        public void SendBlendShapeProxyApply()
        {
            if (IsRunning)
            {
                OnSendBlendShapeProxyApply?.Invoke(new BlendShapeProxyApply(){ Value = true });
                _oscClient.Send(OscAddress.BlendShapeProxyApply);
            }
        }

        public void SendTransportedBlendShapeProxyApply(int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedBlendShapeProxyApply?.Invoke(new BlendShapeProxyApply(){ Value = true }, networkClientId);
                _oscClient.Send(OscAddress.TransportedBlendShapeProxyApply, networkClientId);
            }
        }

        public void SendCamera(Camera camera)
        {
            if (IsRunning)
            {
                OnSendCamera?.Invoke(camera);
                _oscClient.Send(OscAddress.Camera, camera.Name, 
                                camera.PositionX, camera.PositionY, camera.PositionZ, 
                                camera.RotationX, camera.RotationY, camera.RotationZ, camera.RotationW, camera.Fov);
            }
        }

        public void SendTransportedCamera(Camera camera, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedCamera?.Invoke(camera, networkClientId);
                _oscClient.Send(OscAddress.TransportedCamera, camera.Name, 
                                camera.PositionX, camera.PositionY, camera.PositionZ, 
                                camera.RotationX, camera.RotationY, camera.RotationZ, camera.RotationW, camera.Fov, networkClientId);
            }
        }

        public void SendLight(Light light)
        {
            if (IsRunning)
            {
                OnSendLight?.Invoke(light);
                _oscClient.Send(OscAddress.Light, light.Name, 
                                light.PositionX, light.PositionY, light.PositionZ, 
                                light.RotationX, light.RotationY, light.RotationZ, light.RotationW, 
                                light.ColorRed, light.ColorGreen, light.ColorBlue, light.ColorAlpha);
            }
        }

        public void SendTransportedLight(Light light, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedLight?.Invoke(light, networkClientId);
                _oscClient.Send(OscAddress.TransportedLight, light.Name, 
                                light.PositionX, light.PositionY, light.PositionZ, 
                                light.RotationX, light.RotationY, light.RotationZ, light.RotationW, 
                                light.ColorRed, light.ColorGreen, light.ColorBlue, light.ColorAlpha, networkClientId);
            }
        }

        public void SendControllerInput(ControllerInput controllerInput)
        {
            if (IsRunning)
            {
                OnSendControllerInput?.Invoke(controllerInput);
                _oscClient.Send(OscAddress.ControllerInput, controllerInput.Active, controllerInput.Name, 
                                controllerInput.IsLeft, controllerInput.IsTouch, controllerInput.IsAxis, 
                                controllerInput.AxisX, controllerInput.AxisY, controllerInput.AxisZ);
            }
        }

        public void SendTransportedControllerInput(ControllerInput controllerInput, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedControllerInput?.Invoke(controllerInput, networkClientId);
                _oscClient.Send(OscAddress.TransportedControllerInput, controllerInput.Active, controllerInput.Name, 
                                controllerInput.IsLeft, controllerInput.IsTouch, controllerInput.IsAxis, 
                                controllerInput.AxisX, controllerInput.AxisY, controllerInput.AxisZ, networkClientId);
            }
        }

        public void SendKeyInput(KeyInput keyInput)
        {
            if (IsRunning)
            {
                OnSendKeyInput?.Invoke(keyInput);
                _oscClient.Send(OscAddress.KeyInput, keyInput.Active, keyInput.Name, keyInput.Keycode);
            }
        }

        public void SendTransportedKeyInput(KeyInput keyInput, int networkClientId = -1)
        {
            if (networkClientId < 0) return;
            if (IsRunning)
            {
                OnSendTransportedKeyInput?.Invoke(keyInput, networkClientId);
                _oscClient.Send(OscAddress.TransportedKeyInput, keyInput.Active, keyInput.Name, keyInput.Keycode, networkClientId);
            }
        }

        public void SendDeviceTransform(DeviceTransform deviceTransform)
        {
            var messageType = deviceTransform.DeviceType switch
            {
                DeviceType.HeadMountedDisplay => OscAddress.HmdDeviceTransform,
                DeviceType.Controller => OscAddress.ControllerDeviceTransform,
                DeviceType.Tracker => OscAddress.TrackerDeviceTransform,
                _ => OscAddress.TrackerDeviceTransform,
                // _ => throw new ArgumentOutOfRangeException($"Unknown device type: {deviceTransform.DeviceType}"),
            };

            if (IsRunning)
            {
                OnSendDeviceTransform?.Invoke(deviceTransform);
                _oscClient.Send(messageType, deviceTransform.Serial, 
                                deviceTransform.PositionX, deviceTransform.PositionY, deviceTransform.PositionZ, 
                                deviceTransform.RotationX, deviceTransform.RotationY, deviceTransform.RotationZ, deviceTransform.RotationW);
            }
        }

        public void SendTransportedDeviceTransform(DeviceTransform deviceTransform, int networkClientId = -1)
        {
            if (networkClientId < 0) return;

            var messageType = deviceTransform.DeviceType switch
            {
                DeviceType.HeadMountedDisplay => OscAddress.TransportedHmdDeviceTransform,
                DeviceType.Controller => OscAddress.TransportedControllerDeviceTransform,
                DeviceType.Tracker => OscAddress.TransportedTrackerDeviceTransform,
                _ => OscAddress.TransportedTrackerDeviceTransform,
                // _ => throw new ArgumentOutOfRangeException($"Unknown device type: {deviceTransform.DeviceType}"),
            };

            if (IsRunning)
            {
                OnSendTransportedDeviceTransform?.Invoke(deviceTransform, networkClientId);
                _oscClient.Send(messageType, deviceTransform.Serial, 
                                deviceTransform.PositionX, deviceTransform.PositionY, deviceTransform.PositionZ, 
                                deviceTransform.RotationX, deviceTransform.RotationY, deviceTransform.RotationZ, deviceTransform.RotationW, networkClientId);
            }
        }

        public void SendDeviceLocalTransform(DeviceLocalTransform deviceTransform)
        {
            var messageType = deviceTransform.DeviceType switch
            {
                DeviceType.HeadMountedDisplay => OscAddress.HmdDeviceLocalTransform,
                DeviceType.Controller => OscAddress.ControllerDeviceLocalTransform,
                DeviceType.Tracker => OscAddress.TrackerDeviceLocalTransform,
                _ => OscAddress.TrackerDeviceLocalTransform,
                // _ => throw new ArgumentOutOfRangeException($"Unknown device type: {deviceTransform.DeviceType}"),
            };

            if (IsRunning)
            {
                OnSendDeviceLocalTransform?.Invoke(deviceTransform);
                _oscClient.Send(messageType, deviceTransform.Serial, 
                                deviceTransform.PositionX, deviceTransform.PositionY, deviceTransform.PositionZ, 
                                deviceTransform.RotationX, deviceTransform.RotationY, deviceTransform.RotationZ, deviceTransform.RotationW);
            }
        }

        public void SendTransportedDeviceLocalTransform(DeviceLocalTransform deviceTransform, int networkClientId = -1)
        {
            if (networkClientId < 0) return;

            var messageType = deviceTransform.DeviceType switch
            {
                DeviceType.HeadMountedDisplay => OscAddress.TransportedHmdDeviceLocalTransform,
                DeviceType.Controller => OscAddress.TransportedControllerDeviceLocalTransform,
                DeviceType.Tracker => OscAddress.TransportedTrackerDeviceLocalTransform,
                _ => OscAddress.TransportedTrackerDeviceLocalTransform,
                // _ => throw new ArgumentOutOfRangeException($"Unknown device type: {deviceTransform.DeviceType}"),
            };

            if (IsRunning)
            {
                OnSendTransportedDeviceLocalTransform?.Invoke(deviceTransform, networkClientId);
                _oscClient.Send(messageType, deviceTransform.Serial, 
                                deviceTransform.PositionX, deviceTransform.PositionY, deviceTransform.PositionZ, 
                                deviceTransform.RotationX, deviceTransform.RotationY, deviceTransform.RotationZ, deviceTransform.RotationW, networkClientId);
            }
        }
    }
}