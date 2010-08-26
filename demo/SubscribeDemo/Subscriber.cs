using System;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Jackalope;
using System.Linq;

namespace SubscribeDemo
{
    public class Subscriber : IDaemon
    {
        private IBus _bus;

        public void Start()
        {
            "Subscriber is starting.".ToInfo<Subscriber>();
            _bus.Subscribe("subscriber");
        }

        public void Stop()
        {
            "Stop was called."
                .ToInfo<Subscriber>();
        }

        public Subscriber(IBus bus)
        {
            _bus = bus;
            _bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout));
            _bus.AddEndPoint(x => x.QueueName("subscriber"));
            _bus.BindQueue("subscriber", "publisher");

            _bus
                .QueueStreams["subscriber"]
                .BufferWithTime(TimeSpan.FromSeconds(1))
                .Subscribe(x => "Processed {0} messages in 1 second".ToInfo<Subscriber>(x.Count));
        }
    }
}