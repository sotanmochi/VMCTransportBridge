// This code is based on the VMC Protocol specification.
//
// References:
//   - https://protocol.vmc.info/english
//   - https://protocol.vmc.info/marionette-spec
//   - https://protocol.vmc.info/performer-spec
//
using System;
using System.Threading;
using OscJack;

namespace VMCTransportBridge
{
    public sealed class OscMessageReceiver : IMessageReceiver
    {
        public event Action<PerformerAppStatus> OnReceivePerformerAppStatus;
        public event Action<LocalVrm> OnReceiveLocalVrm;
        public event Action<RemoteVrm> OnReceiveRemoteVrm;
        public event Action<Time> OnReceiveTime;
        public event Action<RootTransform> OnReceiveRootTransform;
        public event Action<BoneTransform> OnReceiveBoneTransform;
        public event Action<BlendShapeProxyValue> OnReceiveBlendShapeProxyValue;
        public event Action<BlendShapeProxyApply> OnReceiveBlendShapeProxyApply;
        public event Action<Camera> OnReceiveCamera;
        public event Action<Light> OnReceiveLight;
        public event Action<ControllerInput> OnReceiveControllerInput;
        public event Action<KeyInput> OnReceiveKeyInput;
        public event Action<DeviceTransform> OnReceiveDeviceTransform;
        public event Action<DeviceLocalTransform> OnReceiveDeviceLocalTransform;

        //////////////////////////////////////////////////
        ///  Extensions for network transport bridge   ///
        //////////////////////////////////////////////////
        public event Action<PerformerAppStatus, int> OnReceiveTransportedPerformerAppStatus;
        public event Action<LocalVrm, int> OnReceiveTransportedLocalVrm;
        public event Action<RemoteVrm, int> OnReceiveTransportedRemoteVrm;
        public event Action<Time, int> OnReceiveTransportedTime;
        public event Action<RootTransform, int> OnReceiveTransportedRootTransform;
        public event Action<BoneTransform, int> OnReceiveTransportedBoneTransform;
        public event Action<BlendShapeProxyValue, int> OnReceiveTransportedBlendShapeProxyValue;
        public event Action<BlendShapeProxyApply, int> OnReceiveTransportedBlendShapeProxyApply;
        public event Action<Camera, int> OnReceiveTransportedCamera;
        public event Action<Light, int> OnReceiveTransportedLight;
        public event Action<ControllerInput, int> OnReceiveTransportedControllerInput;
        public event Action<KeyInput, int> OnReceiveTransportedKeyInput;
        public event Action<DeviceTransform, int> OnReceiveTransportedDeviceTransform;
        public event Action<DeviceLocalTransform, int> OnReceiveTransportedDeviceLocalTransform;
        //////////////////////////////////////////////////

        public bool IsRunning => _isRunning;
        public ushort Port => _port;

        private readonly PerformerAppStatus _statusBuffer = new PerformerAppStatus();
        private readonly RootTransform _rootTransformBuffer = new RootTransform();
        private readonly BoneTransform _boneTransformBuffer = new BoneTransform();
        private readonly BlendShapeProxyValue _blendShapeProxyValueBuffer = new BlendShapeProxyValue();
        private readonly Camera _cameraBuffer = new Camera();
        private readonly Light _lightBuffer = new Light();
        private readonly DeviceTransform _deviceTransformBuffer = new DeviceTransform();
        private readonly DeviceLocalTransform _deviceLocalTransformBuffer = new DeviceLocalTransform();

        private readonly SynchronizationContext _syncContext;

        private OscServer _oscServer;
        private bool _isRunning;
        private ushort _port;

        public OscMessageReceiver(ushort defaultPort = 39539, SynchronizationContext syncContext = null)
        {
            _port = defaultPort;
            _syncContext = syncContext;
        }

        public void Start(ushort port)
        {
            _port = port;
            _oscServer = new OscServer(port);
            AddCallbacks();
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
            _oscServer?.Dispose();
            _oscServer = null;
        }

        private void AddCallbacks()
        {
            _oscServer.MessageDispatcher.AddCallback(OscAddress.PerformerAppStatus, OnReceivePerformerAppStatusHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.LocalVrm, OnReceiveLocalVrmHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.RemoteVrm, OnReceiveRemoteVrmHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.Time, OnReceiveTimeHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.RootTransform, OnReceiveRootTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.BoneTransform, OnReceiveBoneTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.BlendShapeProxyValue, OnReceiveBlendShapeProxyValueHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.BlendShapeProxyApply, OnReceiveBlendShapeProxyApplyHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.Camera, OnReceiveCameraHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.Light, OnReceiveLightHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.ControllerInput, OnReceiveControllerInputHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.KeyInput, OnReceiveKeyInputHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.HmdDeviceTransform, OnReceiveDeviceTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.ControllerDeviceTransform, OnReceiveDeviceTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TrackerDeviceTransform, OnReceiveDeviceTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.HmdDeviceLocalTransform, OnReceiveDeviceLocalTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.ControllerDeviceLocalTransform, OnReceiveDeviceLocalTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TrackerDeviceLocalTransform, OnReceiveDeviceLocalTransformHandler);

            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedPerformerAppStatus, OnReceiveTransportedPerformerAppStatusHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedLocalVrm, OnReceiveTransportedLocalVrmHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedRemoteVrm, OnReceiveTransportedRemoteVrmHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedTime, OnReceiveTransportedTimeHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedRootTransform, OnReceiveTransportedRootTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedBoneTransform, OnReceiveTransportedBoneTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedBlendShapeProxyValue, OnReceiveTransportedBlendShapeProxyValueHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedBlendShapeProxyApply, OnReceiveTransportedBlendShapeProxyApplyHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedCamera, OnReceiveTransportedCameraHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedLight, OnReceiveTransportedLightHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedControllerInput, OnReceiveTransportedControllerInputHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedKeyInput, OnReceiveTransportedKeyInputHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedHmdDeviceTransform, OnReceiveTransportedDeviceTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedControllerDeviceTransform, OnReceiveTransportedDeviceTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedTrackerDeviceTransform, OnReceiveTransportedDeviceTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedHmdDeviceLocalTransform, OnReceiveTransportedDeviceLocalTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedControllerDeviceLocalTransform, OnReceiveTransportedDeviceLocalTransformHandler);
            _oscServer.MessageDispatcher.AddCallback(OscAddress.TransportedTrackerDeviceLocalTransform, OnReceiveTransportedDeviceLocalTransformHandler);
        }

        private void OnReceivePerformerAppStatusHandler(string address, OscDataHandle data)
        {
            var status = _statusBuffer;
            var elementCount = data.GetElementCount();

            status.SetDefault();

            // VMC Protocol
            // /VMC/Ext/OK (int){loaded}
            if (elementCount == 1)
            {
                status.Loaded = data.GetElementAsInt(0);
            }
            // VMC Protocol V2.5 or later
            // /VMC/Ext/OK (int){loaded} (int){calibration state} (int){calibration mode}
            else if (elementCount == 3)
            {
                status.Loaded = data.GetElementAsInt(0);
                status.CalibrationState = data.GetElementAsInt(1);
                status.CalibrationMode = data.GetElementAsInt(2);
            }
            // VMC Protocol V2.7 or later
            // /VMC/Ext/OK (int){loaded} (int){calibration state} (int){calibration mode} (int){tracking status}
            else if (elementCount == 4)
            {
                status.Loaded = data.GetElementAsInt(0);
                status.CalibrationState = data.GetElementAsInt(1);
                status.CalibrationMode = data.GetElementAsInt(2);
                status.TrackingStatus = data.GetElementAsInt(3);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceivePerformerAppStatus?.Invoke(status), null);
            }
            else
            {
                OnReceivePerformerAppStatus?.Invoke(status);
            }
        }

        private void OnReceiveTransportedPerformerAppStatusHandler(string address, OscDataHandle data)
        {
            var status = _statusBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            status.SetDefault();

            // Extension for network transport bridge
            // /VMC/Ext/OK/Transported (int){loaded} (int){calibration state} (int){calibration mode} (int){tracking status} (int){NetworkClientId}
            if (elementCount == 5)
            {
                status.Loaded = data.GetElementAsInt(0);
                status.CalibrationState = data.GetElementAsInt(1);
                status.CalibrationMode = data.GetElementAsInt(2);
                status.TrackingStatus = data.GetElementAsInt(3);
                networkClientId = data.GetElementAsInt(4);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedPerformerAppStatus?.Invoke(status, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedPerformerAppStatus?.Invoke(status, networkClientId);
            }
        }

        private void OnReceiveLocalVrmHandler(string address, OscDataHandle data)
        {
            var localVrm = new LocalVrm();
            var elementCount = data.GetElementCount();

            // VMC Protocol V2.4 or later
            // /VMC/Ext/VRM (string){path} (string){title}
            if (elementCount == 2)
            {
                localVrm.Path = data.GetElementAsString(0);
                localVrm.Title = data.GetElementAsString(1);
            }
            // VMC Protocol V2.7 or later
            // /VMC/Ext/VRM (string){path} (string){title} (string){Hash}
            else if (elementCount == 3)
            {
                localVrm.Path = data.GetElementAsString(0);
                localVrm.Title = data.GetElementAsString(1);
                localVrm.Hash = data.GetElementAsString(2);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveLocalVrm?.Invoke(localVrm), null);
            }
            else
            {
                OnReceiveLocalVrm?.Invoke(localVrm);
            }
        }

        private void OnReceiveTransportedLocalVrmHandler(string address, OscDataHandle data)
        {
            var localVrm = new LocalVrm();
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/VRM/Transported (string){path} (string){title} (string){Hash} (int){NetworkClientId}
            if (elementCount == 4)
            {
                localVrm.Path = data.GetElementAsString(0);
                localVrm.Title = data.GetElementAsString(1);
                localVrm.Hash = data.GetElementAsString(2);
                networkClientId = data.GetElementAsInt(3);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedLocalVrm?.Invoke(localVrm, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedLocalVrm?.Invoke(localVrm, networkClientId);
            }
        }

        private void OnReceiveRemoteVrmHandler(string address, OscDataHandle data)
        {
            var remoteVrm = new RemoteVrm();
            var elementCount = data.GetElementCount();

            // VMC Protocol V3.0 or later
            // /VMC/Ext/Remote (string){service} (string){json}
            if (elementCount == 2)
            {
                remoteVrm.ServiceName = data.GetElementAsString(0);
                remoteVrm.Json = data.GetElementAsString(1);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveRemoteVrm?.Invoke(remoteVrm), null);
            }
            else
            {
                OnReceiveRemoteVrm?.Invoke(remoteVrm);
            }
        }

        private void OnReceiveTransportedRemoteVrmHandler(string address, OscDataHandle data)
        {
            var remoteVrm = new RemoteVrm();
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Remote/Transported (string){service} (string){json} (int){NetworkClientId}
            if (elementCount == 3)
            {
                remoteVrm.ServiceName = data.GetElementAsString(0);
                remoteVrm.Json = data.GetElementAsString(1);
                networkClientId = data.GetElementAsInt(2);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedRemoteVrm?.Invoke(remoteVrm, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedRemoteVrm?.Invoke(remoteVrm, networkClientId);
            }
        }

        private void OnReceiveTimeHandler(string address, OscDataHandle data)
        {
            var elementCount = data.GetElementCount();

            // VMC Protocol
            // /VMC/Ext/T (float){time}
            if (elementCount == 1)
            {
                var time = data.GetElementAsFloat(0);

                if (_syncContext != null)
                {
                    _syncContext.Post(_ => OnReceiveTime?.Invoke(new Time(){ Value = time }), null);
                }
                else
                {
                    OnReceiveTime?.Invoke(new Time(){ Value = time });
                }
            }
        }

        private void OnReceiveTransportedTimeHandler(string address, OscDataHandle data)
        {
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/T/Transported (float){time} (int){NetworkClientId}
            if (elementCount == 2)
            {
                var time = data.GetElementAsFloat(0);
                var networkClientId = data.GetElementAsInt(1);

                if (_syncContext != null)
                {
                    _syncContext.Post(_ => OnReceiveTransportedTime?.Invoke(new Time(){ Value = time }, networkClientId), null);
                }
                else
                {
                    OnReceiveTransportedTime?.Invoke(new Time(){ Value = time }, networkClientId);
                }
            }
        }

        private void OnReceiveRootTransformHandler(string address, OscDataHandle data)
        {
            var rootTransform = _rootTransformBuffer;
            var elementCount = data.GetElementCount();

            rootTransform.SetDefault();

            // VMC Protocol V2.0 or later
            // /VMC/Ext/Root/Pos (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w}
            if (elementCount >= 8)
            { 
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);

                rootTransform.Name = name;
                rootTransform.PositionX = px;
                rootTransform.PositionY = py;
                rootTransform.PositionZ = pz;
                rootTransform.RotationX = qx;
                rootTransform.RotationY = qy;
                rootTransform.RotationZ = qz;
                rootTransform.RotationW = qw;
                rootTransform.ScaleX = 1f;
                rootTransform.ScaleY = 1f;
                rootTransform.ScaleZ = 1f;
                rootTransform.OffsetX = 0f;
                rootTransform.OffsetY = 0f;
                rootTransform.OffsetZ = 0f;
            }
            // VMC Protocol V2.1 or later
            // /VMC/Ext/Root/Pos (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} 
            //                   (float){s.x} (float){s.y} (float){s.z} (float){o.x} (float){o.y} (float){o.z}
            if (elementCount >= 14)
            { 
                var sx = data.GetElementAsFloat(8);
                var sy = data.GetElementAsFloat(9);
                var sz = data.GetElementAsFloat(10);
                var ox = data.GetElementAsFloat(11);
                var oy = data.GetElementAsFloat(12);
                var oz = data.GetElementAsFloat(13);

                rootTransform.ScaleX = sx;
                rootTransform.ScaleY = sy;
                rootTransform.ScaleZ = sz;
                rootTransform.OffsetX = ox;
                rootTransform.OffsetY = oy;
                rootTransform.OffsetZ = oz;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveRootTransform?.Invoke(rootTransform), null);
            }
            else
            {
                OnReceiveRootTransform?.Invoke(rootTransform);
            }
        }

        private void OnReceiveTransportedRootTransformHandler(string address, OscDataHandle data)
        {
            var rootTransform = _rootTransformBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            rootTransform.SetDefault();

            // Extension for network transport bridge
            // /VMC/Ext/Root/Pos/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} 
            //                               (float){s.x} (float){s.y} (float){s.z} (float){o.x} (float){o.y} (float){o.z}
            //                               (int){NetworkClientId}
            if (elementCount == 15)
            {
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);
                var sx = data.GetElementAsFloat(8);
                var sy = data.GetElementAsFloat(9);
                var sz = data.GetElementAsFloat(10);
                var ox = data.GetElementAsFloat(11);
                var oy = data.GetElementAsFloat(12);
                var oz = data.GetElementAsFloat(13);

                rootTransform.Name = name;
                rootTransform.PositionX = px;
                rootTransform.PositionY = py;
                rootTransform.PositionZ = pz;
                rootTransform.RotationX = qx;
                rootTransform.RotationY = qy;
                rootTransform.RotationZ = qz;
                rootTransform.RotationW = qw;
                rootTransform.ScaleX = sx;
                rootTransform.ScaleY = sy;
                rootTransform.ScaleZ = sz;
                rootTransform.OffsetX = ox;
                rootTransform.OffsetY = oy;
                rootTransform.OffsetZ = oz;

                networkClientId = data.GetElementAsInt(14);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedRootTransform?.Invoke(rootTransform, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedRootTransform?.Invoke(rootTransform, networkClientId);
            }
        }

        private void OnReceiveBoneTransformHandler(string address, OscDataHandle data)
        {
            var boneTransform = _boneTransformBuffer;
            var elementCount = data.GetElementCount();

            // VMC Protocol
            // /VMC/Ext/Bone/Pos (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w}
            if (elementCount == 8)
            {
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);

                boneTransform.Name = name;
                boneTransform.PositionX = px;
                boneTransform.PositionY = py;
                boneTransform.PositionZ = pz;
                boneTransform.RotationX = qx;
                boneTransform.RotationY = qy;
                boneTransform.RotationZ = qz;
                boneTransform.RotationW = qw;
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveBoneTransform?.Invoke(boneTransform), null);
            }
            else
            {
                OnReceiveBoneTransform?.Invoke(boneTransform);
            }
        }

        private void OnReceiveTransportedBoneTransformHandler(string address, OscDataHandle data)
        {
            var boneTransform = _boneTransformBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Bone/Pos/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w}
            //                               (int){NetworkClientId}
            if (elementCount == 9)
            {
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);

                boneTransform.Name = name;
                boneTransform.PositionX = px;
                boneTransform.PositionY = py;
                boneTransform.PositionZ = pz;
                boneTransform.RotationX = qx;
                boneTransform.RotationY = qy;
                boneTransform.RotationZ = qz;
                boneTransform.RotationW = qw;

                networkClientId = data.GetElementAsInt(8);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedBoneTransform?.Invoke(boneTransform, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedBoneTransform?.Invoke(boneTransform, networkClientId);
            }
        }

        private void OnReceiveBlendShapeProxyValueHandler(string address, OscDataHandle data)
        {
            var blendShapeProxyValue = _blendShapeProxyValueBuffer;
            var elementCount = data.GetElementCount();

            // VMC Protocol
            // /VMC/Ext/Blend/Val (string){name} (float){value}
            if (elementCount == 2)
            {
                blendShapeProxyValue.Name = data.GetElementAsString(0);
                blendShapeProxyValue.Value = data.GetElementAsFloat(1);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveBlendShapeProxyValue?.Invoke(blendShapeProxyValue), null);
            }
            else
            {
                OnReceiveBlendShapeProxyValue?.Invoke(blendShapeProxyValue);
            }
        }

        private void OnReceiveTransportedBlendShapeProxyValueHandler(string address, OscDataHandle data)
        {
            var blendShapeProxyValue = _blendShapeProxyValueBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Blend/Val/Transported (string){name} (float){value} (int){NetworkClientId} 
            if (elementCount == 3)
            {
                blendShapeProxyValue.Name = data.GetElementAsString(0);
                blendShapeProxyValue.Value = data.GetElementAsFloat(1);
                networkClientId = data.GetElementAsInt(2);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedBlendShapeProxyValue?.Invoke(blendShapeProxyValue, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedBlendShapeProxyValue?.Invoke(blendShapeProxyValue, networkClientId);
            }
        }

        private void OnReceiveBlendShapeProxyApplyHandler(string address, OscDataHandle data)
        {
            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveBlendShapeProxyApply?.Invoke(new BlendShapeProxyApply(){ Value = true }), null);
            }
            else
            {
                OnReceiveBlendShapeProxyApply?.Invoke(new BlendShapeProxyApply(){ Value = true });
            }
        }

        private void OnReceiveTransportedBlendShapeProxyApplyHandler(string address, OscDataHandle data)
        {
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Blend/Apply/Transported (int){NetworkClientId} 
            if (elementCount == 1)
            {
                networkClientId = data.GetElementAsInt(0);

                if (_syncContext != null)
                {
                    _syncContext.Post(_ => OnReceiveTransportedBlendShapeProxyApply?.Invoke(new BlendShapeProxyApply(){ Value = true }, networkClientId), null);
                }
                else
                {
                    OnReceiveTransportedBlendShapeProxyApply?.Invoke(new BlendShapeProxyApply(){ Value = true }, networkClientId);
                }
            }
        }

        private void OnReceiveCameraHandler(string address, OscDataHandle data)
        {
            var camera = _cameraBuffer;
            var elementCount = data.GetElementCount();

            // VMC Protocol V2.1 or later
            // /VMC/Ext/Cam (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (float){fov}
            if (elementCount == 9)
            {
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);
                var fov = data.GetElementAsFloat(8);

                camera.Name = name;
                camera.PositionX = px;
                camera.PositionY = py;
                camera.PositionZ = pz;
                camera.RotationX = qx;
                camera.RotationY = qy;
                camera.RotationZ = qz;
                camera.RotationW = qw;
                camera.Fov = fov;
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveCamera?.Invoke(camera), null);
            }
            else
            {
                OnReceiveCamera?.Invoke(camera);
            }
        }

        private void OnReceiveTransportedCameraHandler(string address, OscDataHandle data)
        {
            var camera = _cameraBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Cam/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} (float){fov}
            //                          (int){NetworkClientId}
            if (elementCount == 10)
            {
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);
                var fov = data.GetElementAsFloat(8);

                camera.Name = name;
                camera.PositionX = px;
                camera.PositionY = py;
                camera.PositionZ = pz;
                camera.RotationX = qx;
                camera.RotationY = qy;
                camera.RotationZ = qz;
                camera.RotationW = qw;
                camera.Fov = fov;

                networkClientId = data.GetElementAsInt(9);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedCamera?.Invoke(camera, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedCamera?.Invoke(camera, networkClientId);
            }
        }

        private void OnReceiveLightHandler(string address, OscDataHandle data)
        {
            var light = _lightBuffer;
            var elementCount = data.GetElementCount();

            // VMC Protocol V2.4 or later
            // /VMC/Ext/Light/ (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} 
            //                 (float){color.red} (float){color.green} (float){color.blue} (float){color.alpha}
            if (elementCount == 12)
            {
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);
                var red = data.GetElementAsFloat(8);
                var green = data.GetElementAsFloat(9);
                var blue = data.GetElementAsFloat(10);
                var alpha = data.GetElementAsFloat(11);

                light.Name = name;
                light.PositionX = px;
                light.PositionY = py;
                light.PositionZ = pz;
                light.RotationX = qx;
                light.RotationY = qy;
                light.RotationZ = qz;
                light.RotationW = qw;
                light.ColorRed = red;
                light.ColorGreen = green;
                light.ColorBlue = blue;
                light.ColorAlpha = alpha;
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveLight?.Invoke(light), null);
            }
            else
            {
                OnReceiveLight?.Invoke(light);
            }
        }

        private void OnReceiveTransportedLightHandler(string address, OscDataHandle data)
        {
            var light = _lightBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Light/Transported (string){name} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w} 
            //                            (float){color.red} (float){color.green} (float){color.blue} (float){color.alpha}
            //                            (int){NetworkClientId}
            if (elementCount == 13)
            {
                var name = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);
                var red = data.GetElementAsFloat(8);
                var green = data.GetElementAsFloat(9);
                var blue = data.GetElementAsFloat(10);
                var alpha = data.GetElementAsFloat(11);

                light.Name = name;
                light.PositionX = px;
                light.PositionY = py;
                light.PositionZ = pz;
                light.RotationX = qx;
                light.RotationY = qy;
                light.RotationZ = qz;
                light.RotationW = qw;
                light.ColorRed = red;
                light.ColorGreen = green;
                light.ColorBlue = blue;
                light.ColorAlpha = alpha;

                networkClientId = data.GetElementAsInt(12);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedLight?.Invoke(light, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedLight?.Invoke(light, networkClientId);
            }
        }

        private void OnReceiveControllerInputHandler(string address, OscDataHandle data)
        {
            var controllerInput = new ControllerInput();
            var elementCount = data.GetElementCount();

            // VMC Protocol V2.1 or later
            // /VMC/Ext/Con (int){active} (string){name} (int){IsLeft} (int){IsTouch} (int){IsAxis} (float){Axis.x} (float){Axis.y} (float){Axis.z}
            if (elementCount == 8)
            {
                controllerInput.Active = data.GetElementAsInt(0);
                controllerInput.Name = data.GetElementAsString(1);
                controllerInput.IsLeft = data.GetElementAsInt(2);
                controllerInput.IsTouch = data.GetElementAsInt(3);
                controllerInput.IsAxis = data.GetElementAsInt(4);
                controllerInput.AxisX = data.GetElementAsFloat(5);
                controllerInput.AxisY = data.GetElementAsFloat(6);
                controllerInput.AxisZ = data.GetElementAsFloat(7);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveControllerInput?.Invoke(controllerInput), null);
            }
            else
            {
                OnReceiveControllerInput?.Invoke(controllerInput);
            }
        }

        private void OnReceiveTransportedControllerInputHandler(string address, OscDataHandle data)
        {
            var controllerInput = new ControllerInput();
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Con/Transported (int){active} (string){name} (int){IsLeft} (int){IsTouch} (int){IsAxis} (float){Axis.x} (float){Axis.y} (float){Axis.z} (int){NetworkClientId}
            if (elementCount == 9)
            {
                controllerInput.Active = data.GetElementAsInt(0);
                controllerInput.Name = data.GetElementAsString(1);
                controllerInput.IsLeft = data.GetElementAsInt(2);
                controllerInput.IsTouch = data.GetElementAsInt(3);
                controllerInput.IsAxis = data.GetElementAsInt(4);
                controllerInput.AxisX = data.GetElementAsFloat(5);
                controllerInput.AxisY = data.GetElementAsFloat(6);
                controllerInput.AxisZ = data.GetElementAsFloat(7);
                networkClientId = data.GetElementAsInt(8);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedControllerInput?.Invoke(controllerInput, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedControllerInput?.Invoke(controllerInput, networkClientId);
            }
        }

        private void OnReceiveKeyInputHandler(string address, OscDataHandle data)
        {
            var keyInput = new KeyInput();
            var elementCount = data.GetElementCount();

            // VMC Protocol V2.1 or later
            // /VMC/Ext/Key (int){active} (string){name} (int){keycode}
            if (elementCount == 3)
            {
                keyInput.Active = data.GetElementAsInt(0);
                keyInput.Name = data.GetElementAsString(1);
                keyInput.Keycode = data.GetElementAsInt(2);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveKeyInput?.Invoke(keyInput), null);
            }
            else
            {
                OnReceiveKeyInput?.Invoke(keyInput);
            }
        }

        private void OnReceiveTransportedKeyInputHandler(string address, OscDataHandle data)
        {
            var keyInput = new KeyInput();
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // /VMC/Ext/Key/Transported (int){active} (string){name} (int){keycode} (int){NetworkClientId}
            if (elementCount == 4)
            {
                keyInput.Active = data.GetElementAsInt(0);
                keyInput.Name = data.GetElementAsString(1);
                keyInput.Keycode = data.GetElementAsInt(2);
                networkClientId = data.GetElementAsInt(3);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedKeyInput?.Invoke(keyInput, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedKeyInput?.Invoke(keyInput, networkClientId);
            }
        }

        private void OnReceiveDeviceTransformHandler(string address, OscDataHandle data)
        {
            var type = DeviceType.Unknown;
            if (address == OscAddress.HmdDeviceTransform)
            {
                type = DeviceType.HeadMountedDisplay;
            }
            else if (address == OscAddress.ControllerDeviceTransform)
            {
                type = DeviceType.Controller;
            }
            else if (address == OscAddress.TrackerDeviceTransform)
            {
                type = DeviceType.Tracker;
            }
            else
            {
                return;
            }

            var deviceTransform = _deviceTransformBuffer;
            var elementCount = data.GetElementCount();

            // VMC Protocol V2.2 or later
            // <OscAddress> (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w}
            if (elementCount == 8)
            {
                var serial = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);

                deviceTransform.DeviceType = type;
                deviceTransform.Serial = serial;
                deviceTransform.PositionX = px;
                deviceTransform.PositionY = py;
                deviceTransform.PositionZ = pz;
                deviceTransform.RotationX = qx;
                deviceTransform.RotationY = qy;
                deviceTransform.RotationZ = qz;
                deviceTransform.RotationW = qw;
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveDeviceTransform?.Invoke(deviceTransform), null);
            }
            else
            {
                OnReceiveDeviceTransform?.Invoke(deviceTransform);
            }
        }

        private void OnReceiveTransportedDeviceTransformHandler(string address, OscDataHandle data)
        {
            var type = DeviceType.Unknown;
            if (address == OscAddress.TransportedHmdDeviceTransform)
            {
                type = DeviceType.HeadMountedDisplay;
            }
            else if (address == OscAddress.TransportedControllerDeviceTransform)
            {
                type = DeviceType.Controller;
            }
            else if (address == OscAddress.TransportedTrackerDeviceTransform)
            {
                type = DeviceType.Tracker;
            }
            else
            {
                return;
            }

            var deviceTransform = _deviceTransformBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // <OscAddress> (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w}
            //                  (int){NetworkCliendId}
            if (elementCount == 9)
            {
                var serial = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);

                deviceTransform.DeviceType = type;
                deviceTransform.Serial = serial;
                deviceTransform.PositionX = px;
                deviceTransform.PositionY = py;
                deviceTransform.PositionZ = pz;
                deviceTransform.RotationX = qx;
                deviceTransform.RotationY = qy;
                deviceTransform.RotationZ = qz;
                deviceTransform.RotationW = qw;

                networkClientId = data.GetElementAsInt(8);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedDeviceTransform?.Invoke(deviceTransform, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedDeviceTransform?.Invoke(deviceTransform, networkClientId);
            }
        }

        private void OnReceiveDeviceLocalTransformHandler(string address, OscDataHandle data)
        {
            var type = DeviceType.Unknown;
            if (address == OscAddress.HmdDeviceLocalTransform)
            {
                type = DeviceType.HeadMountedDisplay;
            }
            else if (address == OscAddress.ControllerDeviceLocalTransform)
            {
                type = DeviceType.Controller;
            }
            else if (address == OscAddress.TrackerDeviceLocalTransform)
            {
                type = DeviceType.Tracker;
            }
            else
            {
                return;
            }

            var deviceLocalTransform = _deviceLocalTransformBuffer;
            var elementCount = data.GetElementCount();

            // VMC Protocol V2.3 or later
            // <OscAddress> (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w}
            if (elementCount == 8)
            {
                var serial = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);

                deviceLocalTransform.DeviceType = type;
                deviceLocalTransform.Serial = serial;
                deviceLocalTransform.PositionX = px;
                deviceLocalTransform.PositionY = py;
                deviceLocalTransform.PositionZ = pz;
                deviceLocalTransform.RotationX = qx;
                deviceLocalTransform.RotationY = qy;
                deviceLocalTransform.RotationZ = qz;
                deviceLocalTransform.RotationW = qw;
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveDeviceLocalTransform?.Invoke(deviceLocalTransform), null);
            }
            else
            {
                OnReceiveDeviceLocalTransform?.Invoke(deviceLocalTransform);
            }
        }

        private void OnReceiveTransportedDeviceLocalTransformHandler(string address, OscDataHandle data)
        {
            var type = DeviceType.Unknown;
            if (address == OscAddress.TransportedHmdDeviceLocalTransform)
            {
                type = DeviceType.HeadMountedDisplay;
            }
            else if (address == OscAddress.TransportedControllerDeviceLocalTransform)
            {
                type = DeviceType.Controller;
            }
            else if (address == OscAddress.TransportedTrackerDeviceLocalTransform)
            {
                type = DeviceType.Tracker;
            }
            else
            {
                return;
            }

            var deviceLocalTransform = _deviceLocalTransformBuffer;
            var networkClientId = -1;
            var elementCount = data.GetElementCount();

            // Extension for network transport bridge
            // <OscAddress> (string){serial} (float){p.x} (float){p.y} (float){p.z} (float){q.x} (float){q.y} (float){q.z} (float){q.w}
            //                  (int){NetworkCliendId}
            if (elementCount == 9)
            {
                var serial = data.GetElementAsString(0);
                var px = data.GetElementAsFloat(1);
                var py = data.GetElementAsFloat(2);
                var pz = data.GetElementAsFloat(3);
                var qx = data.GetElementAsFloat(4);
                var qy = data.GetElementAsFloat(5);
                var qz = data.GetElementAsFloat(6);
                var qw = data.GetElementAsFloat(7);

                deviceLocalTransform.DeviceType = type;
                deviceLocalTransform.Serial = serial;
                deviceLocalTransform.PositionX = px;
                deviceLocalTransform.PositionY = py;
                deviceLocalTransform.PositionZ = pz;
                deviceLocalTransform.RotationX = qx;
                deviceLocalTransform.RotationY = qy;
                deviceLocalTransform.RotationZ = qz;
                deviceLocalTransform.RotationW = qw;

                networkClientId = data.GetElementAsInt(8);
            }
            else
            {
                return;
            }

            if (_syncContext != null)
            {
                _syncContext.Post(_ => OnReceiveTransportedDeviceLocalTransform?.Invoke(deviceLocalTransform, networkClientId), null);
            }
            else
            {
                OnReceiveTransportedDeviceLocalTransform?.Invoke(deviceLocalTransform, networkClientId);
            }
        }
    }
}