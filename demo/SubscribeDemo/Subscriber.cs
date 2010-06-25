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
            "Stop was called. S that D."
                .ToInfo<Subscriber>();
        }

        public Subscriber(IBus bus)
        {
            _bus = bus;
            _bus.AddEndPoint(x => x
                                      .Exchange("publisher", ExchangeType.fanout)
                                      .QueueName("subscriber")
                                      .Durable()
                                      .PersistentDelivery());

            _bus
                .QueueMessageStreams["subscriber"]
                .BufferWithTime(TimeSpan.FromSeconds(1))
                .Subscribe(x => "Processed {0} messages in 1 second".ToInfo<Subscriber>(x.Count));
        }
    }
}