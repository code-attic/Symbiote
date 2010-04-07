using Symbiote.Daemon;
using Symbiote.Jackalope;
using Symbiote.Warren;

namespace WarrenService
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

            _bus.AddEndPoint(x => x.Exchange("server", ExchangeType.direct).QueueName("serverQueue"));
            _bus.AddEndPoint(x => x.Exchange("client", ExchangeType.fanout));

            _bus.Subscribe("serverQueue", null);
        }
    }
}