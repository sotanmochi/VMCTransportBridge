using VMCTransportBridge;

namespace VMCMessageReceiver
{
    public class Startup
    {
        private ReceiverContext _receiverContext;
        private OscMessageReceiver _messageReceiver;

        public Startup(ushort port)
        {
            _messageReceiver = new OscMessageReceiver();
            _receiverContext = new ReceiverContext(_messageReceiver);
            _messageReceiver.Start(port);
        }
    }
}