using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using VMCTransportBridge;

namespace VMCMessageSender
{
    public class SenderContext
    {
        private readonly OscMessageSender _messageSender;
        private readonly Thread _updateThread;
        private readonly CancellationTokenSource _cts;
        private readonly StringBuilder _inputMessageBuffer = new StringBuilder();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public SenderContext(OscMessageSender messageSender)
        {
            _messageSender = messageSender;
            _cts = new CancellationTokenSource();
            _updateThread = new Thread(UpdateLoop);
            _updateThread.Start();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _cts?.Cancel();
        }

        private void UpdateLoop()
        {
            while (!_cts.IsCancellationRequested)
            {
                Console.Write("Input command ('send' to send test data, 'q' to quit): ");
                var command = ReadLine().ToLower();

                if (command == "q")
                {
                    _cts?.Cancel();
                }
                else if (command != "send")
                {
                    continue;
                }

                if (command == "send")
                {
                    SendTestData();
                }
            }
        }

        private void SendTestData()
        {
            var elapsedTimeSec = (float)_stopwatch.Elapsed.TotalSeconds;

            Console.WriteLine($"[SenderContext] Elapsed time [sec]: {elapsedTimeSec}");
            Console.WriteLine($"[SenderContext] Send test data.");

            _messageSender.SendTime(elapsedTimeSec);

            _messageSender.SendPerformerAppStatus(new PerformerAppStatus()
            {
                Loaded = -1,
                CalibrationState = -2,
                CalibrationMode = -3,
                TrackingStatus = -4,
            });

            _messageSender.SendLocalVrm(new LocalVrm()
            {
                Path = "TEST.LocalVrm.Path",
                Title = "TEST.LocalVrm.Title",
                Hash = "TEST.LocalVrm.Hash",
            });

            _messageSender.SendRemoteVrm(new RemoteVrm()
            {
                ServiceName = "TEST.RemoteVrm.ServiceName",
                Json = "TEST.RemoteVrm.Json",
            });

            _messageSender.SendRootTransform(new RootTransform()
            {
                PositionX = 1f, PositionY = 2f, PositionZ = 3f,
                RotationX = 4f, RotationY = 5f, RotationZ = 6f, RotationW = 7f,
                ScaleX = 8f, ScaleY = 9f, ScaleZ = 10f, OffsetX = 11f, OffsetY = 12f, OffsetZ = 13f,
            });

            _messageSender.SendBoneTransform(new BoneTransform()
            {
                Name = "TEST.BoneTransform",
                PositionX = 7f, PositionY = 6f, PositionZ = 5f,
                RotationX = 4f, RotationY = 3f, RotationZ = 2f, RotationW = 1f,
            });

            _messageSender.SendBlendShapeProxyValue(new BlendShapeProxyValue()
            {
                Name = "TEST.BlendShapeProxy", Value = 3.14f,
            });
            _messageSender.SendBlendShapeProxyApply();

            _messageSender.SendCamera(new Camera()
            {
                Name = "TEST.Camera", Fov = 60f,
                PositionX = 11f, PositionY = 22f, PositionZ = 33f,
                RotationX = 44f, RotationY = 55f, RotationZ = 66f, RotationW = 77f,
            });
            
            _messageSender.SendLight(new Light()
            {
                Name = "TEST.Light",
                PositionX = 111f, PositionY = 222f, PositionZ = 333f,
                RotationX = 444f, RotationY = 555f, RotationZ = 666f, RotationW = 777f,
                ColorRed = -1f, ColorGreen = -2f, ColorBlue = -3f, ColorAlpha = -4f,
            });

            _messageSender.SendControllerInput(new ControllerInput()
            {
                Active = -1, Name = "TEST.ControllerInput",
                IsLeft = -11, IsTouch = -12, IsAxis = -13, 
                AxisX = -1f, AxisY = -2f, AxisZ = -3f,
            });

            _messageSender.SendKeyInput(new KeyInput()
            {
                Name = "TEST.KeyInput", Active = -1, Keycode = -2,
            });

            _messageSender.SendDeviceTransform(new DeviceTransform()
            {
                Serial = "TEST.DeviceTransform.HMD", DeviceType = DeviceType.HeadMountedDisplay,
                PositionX = 7f, PositionY = 6f, PositionZ = 5f,
                RotationX = 4f, RotationY = 3f, RotationZ = 2f, RotationW = 1f,
            });
            _messageSender.SendDeviceTransform(new DeviceTransform()
            {
                Serial = "TEST.DeviceTransform.Controller", DeviceType = DeviceType.Controller,
                PositionX = 7f, PositionY = 6f, PositionZ = 5f,
                RotationX = 4f, RotationY = 3f, RotationZ = 2f, RotationW = 1f,
            });
            _messageSender.SendDeviceTransform(new DeviceTransform()
            {
                Serial = "TEST.DeviceTransform.Tracker", DeviceType = DeviceType.Tracker,
                PositionX = 7f, PositionY = 6f, PositionZ = 5f,
                RotationX = 4f, RotationY = 3f, RotationZ = 2f, RotationW = 1f,
            });

            _messageSender.SendDeviceLocalTransform(new DeviceLocalTransform()
            {
                Serial = "TEST.DeviceLocalTransform.HMD", DeviceType = DeviceType.HeadMountedDisplay,
                PositionX = 7f, PositionY = 6f, PositionZ = 5f,
                RotationX = 4f, RotationY = 3f, RotationZ = 2f, RotationW = 1f,
            });
            _messageSender.SendDeviceLocalTransform(new DeviceLocalTransform()
            {
                Serial = "TEST.DeviceLocalTransform.Controller", DeviceType = DeviceType.Controller,
                PositionX = 7f, PositionY = 6f, PositionZ = 5f,
                RotationX = 4f, RotationY = 3f, RotationZ = 2f, RotationW = 1f,
            });
            _messageSender.SendDeviceLocalTransform(new DeviceLocalTransform()
            {
                Serial = "TEST.DeviceLocalTransform.Tracker", DeviceType = DeviceType.Tracker,
                PositionX = 7f, PositionY = 6f, PositionZ = 5f,
                RotationX = 4f, RotationY = 3f, RotationZ = 2f, RotationW = 1f,
            });
        }

        private string ReadLine()
        {
            _inputMessageBuffer.Clear();

            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        Console.WriteLine();
                        return string.Empty;

                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return _inputMessageBuffer.ToString();

                    case ConsoleKey.Backspace:
                        if(_inputMessageBuffer.Length > 0)
                        {
                            _inputMessageBuffer.Remove(_inputMessageBuffer.Length - 1, 1);
                            Console.Write("\b \b");
                        }
                        break;

                    default:
                        if(keyInfo.KeyChar != '\0')
                            _inputMessageBuffer.Append(keyInfo.KeyChar);
                        Console.Write(keyInfo.KeyChar);
                        break;
                }
            }
        }
    }
}