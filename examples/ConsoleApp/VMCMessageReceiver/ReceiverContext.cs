using System;
using System.Collections.Generic;
using System.Threading;
using VMCTransportBridge;

namespace VMCMessageReceiver
{
    public class ReceiverContext : IDisposable
    {
        private readonly OscMessageReceiver _receiver;
        private readonly Thread _updateThread;
        private readonly CancellationTokenSource _cts;

        private readonly Dictionary<string, uint> _processedMessageCounts = new Dictionary<string, uint>();

        public ReceiverContext(OscMessageReceiver receiver)
        {
            _processedMessageCounts["PerformerAppStatus"] = 0;
            _processedMessageCounts["LocalVRM"] = 0;
            _processedMessageCounts["RemoteVRM"] = 0;
            _processedMessageCounts["Time"] = 0;
            _processedMessageCounts["RootTransform"] = 0;
            _processedMessageCounts["BoneTransform"] = 0;
            _processedMessageCounts["BlendShapeProxyValue"] = 0;
            _processedMessageCounts["BlendShapeProxyApply"] = 0;
            _processedMessageCounts["Camera"] = 0;
            _processedMessageCounts["Light"] = 0;
            _processedMessageCounts["ControllerInput"] = 0;
            _processedMessageCounts["KeyInput"] = 0;
            _processedMessageCounts["DeviceTransform"] = 0;
            _processedMessageCounts["DeviceLocalTransform"] = 0;

            _receiver = receiver;

            _receiver.OnReceivePerformerAppStatus += OnReceivePerformerAppStatus;
            _receiver.OnReceiveLocalVrm += OnReceiveLocalVrm;
            _receiver.OnReceiveRemoteVrm += OnReceiveRemoteVrm;
            _receiver.OnReceiveTime += OnReceiveTime;
            _receiver.OnReceiveRootTransform += OnReceiveRootTransform;
            _receiver.OnReceiveBoneTransform += OnReceiveBoneTransform;
            _receiver.OnReceiveBlendShapeProxyValue += OnReceiveBlendShapeProxyValue;
            _receiver.OnReceiveBlendShapeProxyApply += OnReceiveBlendShapeProxyApply;
            _receiver.OnReceiveCamera += OnReceiveCamera;
            _receiver.OnReceiveLight += OnReceiveLight;
            _receiver.OnReceiveControllerInput += OnReceiveControllerInput;
            _receiver.OnReceiveKeyInput += OnReceiveKeyInput;
            _receiver.OnReceiveDeviceTransform += OnReceiveDeviceTransform;
            _receiver.OnReceiveDeviceLocalTransform += OnReceiveDeviceLocalTransform;

            _cts = new CancellationTokenSource();
            _updateThread = new Thread(UpdateLoop);
            _updateThread.Start();
        }

        public void Dispose()
        {
            _cts?.Dispose();

            _receiver.OnReceivePerformerAppStatus -= OnReceivePerformerAppStatus;
            _receiver.OnReceiveLocalVrm -= OnReceiveLocalVrm;
            _receiver.OnReceiveRemoteVrm -= OnReceiveRemoteVrm;
            _receiver.OnReceiveTime -= OnReceiveTime;
            _receiver.OnReceiveRootTransform -= OnReceiveRootTransform;
            _receiver.OnReceiveBoneTransform -= OnReceiveBoneTransform;
            _receiver.OnReceiveBlendShapeProxyValue -= OnReceiveBlendShapeProxyValue;
            _receiver.OnReceiveBlendShapeProxyApply -= OnReceiveBlendShapeProxyApply;
            _receiver.OnReceiveCamera -= OnReceiveCamera;
            _receiver.OnReceiveLight -= OnReceiveLight;
            _receiver.OnReceiveControllerInput -= OnReceiveControllerInput;
            _receiver.OnReceiveKeyInput -= OnReceiveKeyInput;
            _receiver.OnReceiveDeviceTransform -= OnReceiveDeviceTransform;
            _receiver.OnReceiveDeviceLocalTransform -= OnReceiveDeviceLocalTransform;
        }

        private void UpdateLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                Update();
                Thread.Sleep(3000);
            }
        }

        private void Update()
        {
            var performerAppStatusMessageCounts = _processedMessageCounts["PerformerAppStatus"];
            var localVrmMessageCounts = _processedMessageCounts["LocalVRM"];
            var remoteVrmMessageCounts = _processedMessageCounts["RemoteVRM"];
            var timeMessageCounts = _processedMessageCounts["Time"];
            var rootTransformMessageCounts = _processedMessageCounts["RootTransform"];
            var boneTransformMessageCounts = _processedMessageCounts["BoneTransform"];
            var blendShapeProxyValueMessageCounts = _processedMessageCounts["BlendShapeProxyValue"];
            var blendShapeProxyApplyMessageCounts = _processedMessageCounts["BlendShapeProxyApply"];
            var cameraMessageCounts = _processedMessageCounts["Camera"];
            var lightMessageCounts = _processedMessageCounts["Light"];
            var controllerInputMessageCounts = _processedMessageCounts["ControllerInput"];
            var keyInputMessageCounts = _processedMessageCounts["KeyInput"];
            var deviceTransformMessageCounts = _processedMessageCounts["DeviceTransform"];
            var deviceLocalTransformMessageCounts = _processedMessageCounts["DeviceLocalTransform"];

            Console.WriteLine($"--------------------------------------------------");
            Console.WriteLine($"OscPort: {_receiver.Port}");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"--------------------------------------------------");
            Console.WriteLine($"Received message counts");
            Console.WriteLine($"--------------------------------------------------");
            Console.WriteLine($"PerformerAppStatus: {performerAppStatusMessageCounts}");
            Console.WriteLine($"LocalVRM: {localVrmMessageCounts}");
            Console.WriteLine($"RemoteVRM: {remoteVrmMessageCounts}");
            Console.WriteLine($"Time: {timeMessageCounts}");
            Console.WriteLine($"RootTransform: {rootTransformMessageCounts}");
            Console.WriteLine($"BoneTransform: {boneTransformMessageCounts}");
            Console.WriteLine($"BlendShapeProxyValue: {blendShapeProxyValueMessageCounts}");
            Console.WriteLine($"BlendShapeProxyApply: {blendShapeProxyApplyMessageCounts}");
            Console.WriteLine($"Camera: {cameraMessageCounts}");
            Console.WriteLine($"Light: {lightMessageCounts}");
            Console.WriteLine($"ControllerInput: {controllerInputMessageCounts}");
            Console.WriteLine($"KeyInput: {keyInputMessageCounts}");
            Console.WriteLine($"DeviceTransform: {deviceTransformMessageCounts}");
            Console.WriteLine($"DeviceLocalTransform: {deviceLocalTransformMessageCounts}");
            Console.WriteLine($"");
        }

        private void OnReceivePerformerAppStatus(PerformerAppStatus value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceivePerformerAppStatus");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Loaded: {value.Loaded}");
            Console.WriteLine($"CalibrationState: {value.CalibrationState}");
            Console.WriteLine($"CalibrationMode: {value.CalibrationMode}");
            Console.WriteLine($"TrackingStatus: {value.TrackingStatus}");

            _processedMessageCounts["PerformerAppStatus"] += 1;
        }

        private void OnReceiveLocalVrm(LocalVrm value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveLocalVrm");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Path: {value.Path}");
            Console.WriteLine($"Title: {value.Title}");
            Console.WriteLine($"Hash: {value.Hash}");

            _processedMessageCounts["LocalVRM"] += 1;
        }

        private void OnReceiveRemoteVrm(RemoteVrm value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveRemoteVrm");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ServiceName: {value.ServiceName}");
            Console.WriteLine($"Json: {value.Json}");

            _processedMessageCounts["RemoteVRM"] += 1;
        }

        private void OnReceiveTime(Time value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveTime");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Time: {value.Value}");
            
            _processedMessageCounts["Time"] += 1;
        }

        private void OnReceiveRootTransform(RootTransform value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveRootTransform");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Name: {value.Name}");
            Console.WriteLine($"Position: ({value.PositionX}, {value.PositionY}, {value.PositionZ})");
            Console.WriteLine($"Rotation: ({value.RotationX}, {value.RotationY}, {value.RotationZ}, {value.RotationW})");
            Console.WriteLine($"Scale: ({value.ScaleX}, {value.ScaleY}, {value.ScaleZ})");
            Console.WriteLine($"Offset: ({value.OffsetX}, {value.OffsetY}, {value.OffsetZ})");

            _processedMessageCounts["RootTransform"] += 1;
        }

        private void OnReceiveBoneTransform(BoneTransform value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveBoneTransform");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Name: {value.Name}");
            Console.WriteLine($"Position: ({value.PositionX}, {value.PositionY}, {value.PositionZ})");
            Console.WriteLine($"Rotation: ({value.RotationX}, {value.RotationY}, {value.RotationZ}, {value.RotationW})");

            _processedMessageCounts["BoneTransform"] += 1;
        }

        private void OnReceiveBlendShapeProxyValue(BlendShapeProxyValue value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveBlendShapeProxyValue");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Name: {value.Name}");
            Console.WriteLine($"Value: {value.Value}");
            
            _processedMessageCounts["BlendShapeProxyValue"] += 1;
        }

        private void OnReceiveBlendShapeProxyApply(BlendShapeProxyApply value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveBlendShapeProxyApply");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            
            _processedMessageCounts["BlendShapeProxyApply"] += 1;
        }

        private void OnReceiveCamera(Camera value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveCamera");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Name: {value.Name}");
            Console.WriteLine($"Position: ({value.PositionX}, {value.PositionY}, {value.PositionZ})");
            Console.WriteLine($"Rotation: ({value.RotationX}, {value.RotationY}, {value.RotationZ}, {value.RotationW})");
            Console.WriteLine($"Fov: {value.Fov}");

            _processedMessageCounts["Camera"] += 1;
        }

        private void OnReceiveLight(Light value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveLight");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Name: {value.Name}");
            Console.WriteLine($"Position: ({value.PositionX}, {value.PositionY}, {value.PositionZ})");
            Console.WriteLine($"Rotation: ({value.RotationX}, {value.RotationY}, {value.RotationZ}, {value.RotationW})");
            Console.WriteLine($"Color: ({value.ColorRed}, {value.ColorGreen}, {value.ColorBlue}, {value.ColorAlpha})");

            _processedMessageCounts["Light"] += 1;
        }

        private void OnReceiveControllerInput(ControllerInput value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveControllerInput");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Name: {value.Name}");
            Console.WriteLine($"Active: {value.Active}");
            Console.WriteLine($"(IsLeft, IsTouch, IsAxis): ({value.IsLeft}, {value.IsTouch}, {value.IsAxis})");
            Console.WriteLine($"Axis: ({value.AxisX}, {value.AxisY}, {value.AxisZ})");

            _processedMessageCounts["ControllerInput"] += 1;
        }

        private void OnReceiveKeyInput(KeyInput value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveKeyInput");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Name: {value.Name}");
            Console.WriteLine($"Active: {value.Active}");
            Console.WriteLine($"Keycode: {value.Keycode}");

            _processedMessageCounts["KeyInput"] += 1;
        }

        private void OnReceiveDeviceTransform(DeviceTransform value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveDeviceTransform");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"DeviceType: {value.DeviceType}");
            Console.WriteLine($"Serial: {value.Serial}");
            Console.WriteLine($"Position: ({value.PositionX}, {value.PositionY}, {value.PositionZ})");
            Console.WriteLine($"Rotation: ({value.RotationX}, {value.RotationY}, {value.RotationZ}, {value.RotationW})");

            _processedMessageCounts["DeviceTransform"] += 1;
        }

        private void OnReceiveDeviceLocalTransform(DeviceLocalTransform value)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"OnReceiveDeviceLocalTransform");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"ThreadId: {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"DeviceType: {value.DeviceType}");
            Console.WriteLine($"Serial: {value.Serial}");
            Console.WriteLine($"Position: ({value.PositionX}, {value.PositionY}, {value.PositionZ})");
            Console.WriteLine($"Rotation: ({value.RotationX}, {value.RotationY}, {value.RotationZ}, {value.RotationW})");

            _processedMessageCounts["DeviceLocalTransform"] += 1;
        }
    }
}
