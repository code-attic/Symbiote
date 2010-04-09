using Symbiote.Daemon;
using Symbiote.Jackalope;
using Symbiote.SocketMQ;
using Symbiote.SocketMQ.Impl;

namespace SocketMQ.ChatService
{
    public class ChatService : IDaemon
    {
        protected IMessageGateway _messageGateway;
        protected IBus _bus;

        public void Start()
        {
            _messageGateway.Start();
        }

        public void Stop()
        {
            _messageGateway.Stop();
        }

        public ChatService(IMessageGateway messageGateway, IBus bus)
        {
            _messageGateway = messageGateway;
            _bus = bus;
            _bus.AddEndPoint(x => x.Exchange("client", ExchangeType.fanout));
        }
    }
}