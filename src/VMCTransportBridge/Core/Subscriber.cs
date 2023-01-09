using System;
using System.Buffers;
using System.Threading.Tasks;
using MessagePack;
using VMCTransportBridge.Serialization;

namespace VMCTransportBridge
{
    public sealed class Subscriber : IDisposable
    {
        public delegate void MessageHandler(int messageId, int networkCliendId, ArraySegment<byte> serializedMessage);
        public event MessageHandler OnReceiveTransportedMessage;

        public bool MessageFilterIsEnabled => _messageFilterIsEnabled;
        public int MessageFilterClientId => _clientId;

        private readonly ITransport _transport;
        private readonly IMessageSerializer _messageSerializer;
        private readonly IMessageSender _messageSender;

        private bool _messageFilterIsEnabled;
        private int _clientId = -1;

        public Subscriber(ITransport transport, IMessageSerializer messageSerializer, IMessageSender messageSender)
        {
            _transport = transport;
            _messageSerializer = messageSerializer;
            _messageSender = messageSender;

            _transport.OnReceiveMessage += OnReceiveTransportedMessageEventHandler;
        }

        public void Dispose()
        {
            _transport.OnReceiveMessage -= OnReceiveTransportedMessageEventHandler;
        }

        public void EnableMessageFilter(int networkClientId)
        {
            _messageFilterIsEnabled = true;
            _clientId = networkClientId;
        }

        public void DisableMessageFilter()
        {
            _messageFilterIsEnabled = false;
            _clientId = -1;
        }

        private void OnReceiveTransportedMessageEventHandler(byte[] serializedMessage)
        {
            var messagePackReader = new MessagePackReader(serializedMessage);
            
            var arrayLength = messagePackReader.ReadArrayHeader();
            if (arrayLength != 3)
            {
                throw new InvalidOperationException($"[VMCTransportBridge.Subscriber] ArrayLength: {arrayLength}");
            }
            
            var messageId = messagePackReader.ReadInt32();
            var networkClientId = messagePackReader.ReadInt32();
            var offset = (int)messagePackReader.Consumed;

            OnReceiveTransportedMessage?.Invoke(messageId, networkClientId, new ArraySegment<byte>(serializedMessage, offset, serializedMessage.Length - offset));
            
            if (messageId == (int)MessageType.PerformerAppStatus)
            {
                var value = _messageSerializer.Deserialize<PerformerAppStatus>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceivePerformerAppStatusHandler(value, networkClientId);
            }
            else
            if (messageId == (int)MessageType.LocalVrm)
            {
                var value = _messageSerializer.Deserialize<LocalVrm>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveLocalVrmHandler(value, networkClientId);
            }
            else
            if (messageId == (int)MessageType.RemoteVrm)
            {
                var value = _messageSerializer.Deserialize<RemoteVrm>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveRemoteVrmHandler(value, networkClientId);
            }
            else
            if (messageId == (int)MessageType.Time)
            {
                var value = _messageSerializer.Deserialize<Time>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveTimeHandler(value, networkClientId);
            }
            else
            if (messageId == (int)MessageType.RootTransform)
            {
                var value = _messageSerializer.Deserialize<RootTransform>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveRootTransformHandler(value, networkClientId);
            }
            else
            if (messageId == (int)MessageType.BoneTransform)
            {
                var value = _messageSerializer.Deserialize<BoneTransform>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveBoneTransformHandler(value, networkClientId);
            }
            else
            if (messageId == (int)MessageType.BlendShapeProxyValue)
            {
                var value = _messageSerializer.Deserialize<BlendShapeProxyValue>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveBlendShapeProxyValueHandler(value, networkClientId);                
            }
            else
            if (messageId == (int)MessageType.BlendShapeProxyApply)
            {
                var value = _messageSerializer.Deserialize<BlendShapeProxyApply>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveBlendShapeProxyApplyHandler(networkClientId); 
            }
            else
            if (messageId == (int)MessageType.Camera)
            {
                var value = _messageSerializer.Deserialize<Camera>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveCameraHandler(value, networkClientId);  
            }
            else
            if (messageId == (int)MessageType.Light)
            {
                var value = _messageSerializer.Deserialize<Light>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveLightHandler(value, networkClientId);  
            }
            else
            if (messageId == (int)MessageType.ControllerInput)
            {
                var value = _messageSerializer.Deserialize<ControllerInput>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveControllerInputHandler(value, networkClientId);  
            }
            else
            if (messageId == (int)MessageType.KeyInput)
            {
                var value = _messageSerializer.Deserialize<KeyInput>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveKeyInputHandler(value, networkClientId);  
            }
            else
            if (messageId == (int)MessageType.DeviceTransform)
            {
                var value = _messageSerializer.Deserialize<DeviceTransform>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveDeviceTransformHandler(value, networkClientId);  
            }
            else
            if (messageId == (int)MessageType.DeviceLocalTransform)
            {
                var value = _messageSerializer.Deserialize<DeviceLocalTransform>(new ReadOnlySequence<byte>(serializedMessage, offset, serializedMessage.Length - offset));
                OnReceiveDeviceLocalTransformHandler(value, networkClientId);  
            }
        }

        private void OnReceivePerformerAppStatusHandler(PerformerAppStatus status, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendPerformerAppStatus(status);
            }

            _messageSender.SendTransportedPerformerAppStatus(status, networkClientId);
        }

        private void OnReceiveLocalVrmHandler(LocalVrm localVrm, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendLocalVrm(localVrm);
            }

            _messageSender.SendTransportedLocalVrm(localVrm, networkClientId);
        }

        private void OnReceiveRemoteVrmHandler(RemoteVrm remoteVrm, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendRemoteVrm(remoteVrm);
            }

            _messageSender.SendTransportedRemoteVrm(remoteVrm, networkClientId);
        }

        private void OnReceiveTimeHandler(Time time, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendTime(time.Value);
            }

            _messageSender.SendTransportedTime(time.Value, networkClientId);
        }

        private void OnReceiveRootTransformHandler(RootTransform value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendRootTransform(value);
            }

            _messageSender.SendTransportedRootTransform(value, networkClientId);
        }

        private void OnReceiveBoneTransformHandler(BoneTransform value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendBoneTransform(value);
            }

            _messageSender.SendTransportedBoneTransform(value, networkClientId);
        }

        private void OnReceiveBlendShapeProxyValueHandler(BlendShapeProxyValue value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendBlendShapeProxyValue(value);
            }

            _messageSender.SendTransportedBlendShapeProxyValue(value, networkClientId);
        }

        private void OnReceiveBlendShapeProxyApplyHandler(int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendBlendShapeProxyApply();
            }

            _messageSender.SendTransportedBlendShapeProxyApply(networkClientId);
        }

        private void OnReceiveCameraHandler(Camera value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendCamera(value);
            }

            _messageSender.SendTransportedCamera(value, networkClientId);
        }

        private void OnReceiveLightHandler(Light value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendLight(value);
            }

            _messageSender.SendTransportedLight(value, networkClientId);
        }

        private void OnReceiveControllerInputHandler(ControllerInput value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendControllerInput(value);
            }

            _messageSender.SendTransportedControllerInput(value, networkClientId);
        }

        private void OnReceiveKeyInputHandler(KeyInput value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendKeyInput(value);
            }

            _messageSender.SendTransportedKeyInput(value, networkClientId);
        }

        private void OnReceiveDeviceTransformHandler(DeviceTransform value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendDeviceTransform(value);
            }

            _messageSender.SendTransportedDeviceTransform(value, networkClientId);
        }

        private void OnReceiveDeviceLocalTransformHandler(DeviceLocalTransform value, int networkClientId)
        {
            if (_messageFilterIsEnabled && (_clientId == networkClientId))
            {
                _messageSender.SendDeviceLocalTransform(value);
            }

            _messageSender.SendTransportedDeviceLocalTransform(value, networkClientId);
        }
    }
}