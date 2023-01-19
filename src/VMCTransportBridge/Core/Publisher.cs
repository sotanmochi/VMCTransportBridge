using System;
using System.Buffers;
using System.Threading.Tasks;
using MessagePack;
using VMCTransportBridge.Serialization;
using VMCTransportBridge.Utils;

namespace VMCTransportBridge
{
    public sealed class Publisher : IDisposable
    {
        public delegate void MessageHandler(int messageId, ArraySegment<byte> serializedMessage);
        public event MessageHandler OnSendMessage;

        private readonly ITransport _transport;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMessageReceiver _messageReceiver;

        public Publisher(ITransport transport, IMessageSerializer messageSerializer, IMessageReceiver messageReceiver)
        {
            _transport = transport;
            _messageSerializer = messageSerializer;
            _messageReceiver = messageReceiver;

            _messageReceiver.OnReceivePerformerAppStatus += OnReceivePerformerAppStatusEventHandler;
            _messageReceiver.OnReceiveLocalVrm += OnReceiveLocalVrmEventHandler;
            _messageReceiver.OnReceiveRemoteVrm += OnReceiveRemoteVrmEventHandler;
            _messageReceiver.OnReceiveTime += OnReceiveTimeEventHandler;
            _messageReceiver.OnReceiveRootTransform += OnReceiveRootTransformEventHandler;
            _messageReceiver.OnReceiveBoneTransform += OnReceiveBoneTransformEventHandler;
            _messageReceiver.OnReceiveBlendShapeProxyValue += OnReceiveBlendShapeProxyValueEventHandler;
            _messageReceiver.OnReceiveBlendShapeProxyApply += OnReceiveBlendShapeProxyApplyEventHandler;
            _messageReceiver.OnReceiveCamera += OnReceiveCameraEventHandler;
            _messageReceiver.OnReceiveLight += OnReceiveLightEventHandler;
            _messageReceiver.OnReceiveControllerInput += OnReceiveControllerInputEventHandler;
            _messageReceiver.OnReceiveKeyInput += OnReceiveKeyInputEventHandler;
            _messageReceiver.OnReceiveDeviceTransform += OnReceiveDeviceTransformEventHandler;
            _messageReceiver.OnReceiveDeviceLocalTransform += OnReceiveDeviceLocalTransformEventHandler;
        }

        public void Dispose()
        {
            _messageReceiver.OnReceivePerformerAppStatus -= OnReceivePerformerAppStatusEventHandler;
            _messageReceiver.OnReceiveLocalVrm -= OnReceiveLocalVrmEventHandler;
            _messageReceiver.OnReceiveRemoteVrm -= OnReceiveRemoteVrmEventHandler;
            _messageReceiver.OnReceiveTime -= OnReceiveTimeEventHandler;
            _messageReceiver.OnReceiveRootTransform -= OnReceiveRootTransformEventHandler;
            _messageReceiver.OnReceiveBoneTransform -= OnReceiveBoneTransformEventHandler;
            _messageReceiver.OnReceiveBlendShapeProxyValue -= OnReceiveBlendShapeProxyValueEventHandler;
            _messageReceiver.OnReceiveBlendShapeProxyApply -= OnReceiveBlendShapeProxyApplyEventHandler;
            _messageReceiver.OnReceiveCamera -= OnReceiveCameraEventHandler;
            _messageReceiver.OnReceiveLight -= OnReceiveLightEventHandler;
            _messageReceiver.OnReceiveControllerInput -= OnReceiveControllerInputEventHandler;
            _messageReceiver.OnReceiveKeyInput -= OnReceiveKeyInputEventHandler;
            _messageReceiver.OnReceiveDeviceTransform -= OnReceiveDeviceTransformEventHandler;
            _messageReceiver.OnReceiveDeviceLocalTransform -= OnReceiveDeviceLocalTransformEventHandler;
        }

        public void Publish<T>(T message)
        {
            var messageId = message switch
            {
                PerformerAppStatus   => (int)MessageType.PerformerAppStatus,
                LocalVrm             => (int)MessageType.LocalVrm,
                RemoteVrm            => (int)MessageType.RemoteVrm,
                Time                 => (int)MessageType.Time,
                RootTransform        => (int)MessageType.RootTransform,
                BoneTransform        => (int)MessageType.BoneTransform,
                BlendShapeProxyValue => (int)MessageType.BlendShapeProxyValue,
                BlendShapeProxyApply => (int)MessageType.BlendShapeProxyApply,
                Camera               => (int)MessageType.Camera,
                Light                => (int)MessageType.Light,
                ControllerInput      => (int)MessageType.ControllerInput,
                KeyInput             => (int)MessageType.KeyInput,
                DeviceTransform      => (int)MessageType.DeviceTransform,
                DeviceLocalTransform => (int)MessageType.DeviceLocalTransform,
                _ => -1,
            };

            var transportClientId = _transport.ClientId;

            byte[] SerializeMessage()
            {
                using (var buffer = ArrayPoolBufferWriter.RentThreadStaticWriter())
                {
                    var writer = new MessagePackWriter(buffer);
                    writer.WriteArrayHeader(3);
                    writer.Write(messageId);
                    writer.Write(transportClientId);
                    writer.Flush();
                    _messageSerializer.Serialize(buffer, message);
                    return buffer.WrittenSpan.ToArray();
                }
            }

            var serializedMessage = SerializeMessage();

            OnSendMessage?.Invoke(messageId, serializedMessage);
            _transport.Send(serializedMessage);
        }

        private void OnReceivePerformerAppStatusEventHandler(PerformerAppStatus value)
        {
            Publish<PerformerAppStatus>(value);
        }

        private void OnReceiveLocalVrmEventHandler(LocalVrm value)
        {
            Publish<LocalVrm>(value);
        }

        private void OnReceiveRemoteVrmEventHandler(RemoteVrm value)
        {
            Publish<RemoteVrm>(value);
        }

        private void OnReceiveTimeEventHandler(Time value)
        {
            Publish<Time>(value);
        }

        private void OnReceiveRootTransformEventHandler(RootTransform value)
        {
            Publish<RootTransform>(value);
        }

        private void OnReceiveBoneTransformEventHandler(BoneTransform value)
        {
            Publish<BoneTransform>(value);
        }

        private void OnReceiveBlendShapeProxyValueEventHandler(BlendShapeProxyValue value)
        {
            Publish<BlendShapeProxyValue>(value);
        }

        private void OnReceiveBlendShapeProxyApplyEventHandler(BlendShapeProxyApply value)
        {
            Publish<BlendShapeProxyApply>(value);
        }

        private void OnReceiveCameraEventHandler(Camera value)
        {
            Publish<Camera>(value);
        }

        private void OnReceiveLightEventHandler(Light value)
        {
            Publish<Light>(value);
        }

        private void OnReceiveControllerInputEventHandler(ControllerInput value)
        {
            Publish<ControllerInput>(value);
        }

        private void OnReceiveKeyInputEventHandler(KeyInput value)
        {
            Publish<KeyInput>(value);
        }

        private void OnReceiveDeviceTransformEventHandler(DeviceTransform value)
        {
            Publish<DeviceTransform>(value);
        }

        private void OnReceiveDeviceLocalTransformEventHandler(DeviceLocalTransform value)
        {
            Publish<DeviceLocalTransform>(value);
        }
    }
}