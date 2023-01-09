using VMCTransportBridge;

namespace VMCMessageSender
{
    public class Startup
    {
        private SenderContext _senderContext;
        private OscMessageSender _messageSender;

        public Startup(string destinationAddress, ushort destinationPort)
        {
            _messageSender = new OscMessageSender();
            _senderContext = new SenderContext(_messageSender);
            _messageSender.Start(destinationAddress, destinationPort);
        }

    }
}