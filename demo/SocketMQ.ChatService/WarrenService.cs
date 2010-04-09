using Symbiote.Daemon;
using Symbiote.Jackalope;
using Symbiote.SocketMQ;

namespace SocketMQ.ChatService
{
    public class WarrenService : IDaemon
    {
        protected Bridge _bridge;
        protected IBus _bus;

        public void Start()
        {
            _bridge.Start();
        }

        public void Stop()
        {
            
        }

        public WarrenService(Bridge bridge, IBus bus)
        {
            _bridge = bridge;
            _bus = bus;
            _bus.AddEndPoint(x => x.Exchange("client", ExchangeType.fanout));
        }
    }
}