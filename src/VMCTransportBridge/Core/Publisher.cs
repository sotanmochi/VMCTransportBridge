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

        public async Task PublishAsync<T>(T message)
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
            await _transport.SendAsync(serializedMessage);
        }

        private void OnReceivePerformerAppStatusEventHandler(PerformerAppStatus value)
        {
            PublishAsync<PerformerAppStatus>(value);
        }

        private void OnReceiveLocalVrmEventHandler(LocalVrm value)
        {
            PublishAsync<LocalVrm>(value);
        }

        private void OnReceiveRemoteVrmEventHandler(RemoteVrm value)
        {
            PublishAsync<RemoteVrm>(value);
        }

        private void OnReceiveTimeEventHandler(Time value)
        {
            PublishAsync<Time>(value);
        }

        private void OnReceiveRootTransformEventHandler(RootTransform value)
        {
            PublishAsync<RootTransform>(value);
        }

        private void OnReceiveBoneTransformEventHandler(BoneTransform value)
        {
            PublishAsync<BoneTransform>(value);
        }

        private void OnReceiveBlendShapeProxyValueEventHandler(BlendShapeProxyValue value)
        {
            PublishAsync<BlendShapeProxyValue>(value);
        }

        private void OnReceiveBlendShapeProxyApplyEventHandler(BlendShapeProxyApply value)
        {
            PublishAsync<BlendShapeProxyApply>(value);
        }

        private void OnReceiveCameraEventHandler(Camera value)
        {
            PublishAsync<Camera>(value);
        }

        private void OnReceiveLightEventHandler(Light value)
        {
            PublishAsync<Light>(value);
        }

        private void OnReceiveControllerInputEventHandler(ControllerInput value)
        {
            PublishAsync<ControllerInput>(value);
        }

        private void OnReceiveKeyInputEventHandler(KeyInput value)
        {
            PublishAsync<KeyInput>(value);
        }

        private void OnReceiveDeviceTransformEventHandler(DeviceTransform value)
        {
            PublishAsync<DeviceTransform>(value);
        }

        private void OnReceiveDeviceLocalTransformEventHandler(DeviceLocalTransform value)
        {
            PublishAsync<DeviceLocalTransform>(value);
        }
    }
}