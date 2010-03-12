using System;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Jackalope;

namespace SubscribeDemo
{
    public class Subscriber : IDaemon
    {
        private IBus _bus;

        public void Start()
        {
            "Subscriber is starting.".ToInfo<Subscriber>();
            _bus.Subscribe("subscriber", 1, FailWhale);
        }

        public void Stop()
        {
            "Stop was called. S that D."
                .ToInfo<Subscriber>();
        }

        public void FailWhale(object status)
        {
            "Subscriber is dead. Boo."
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
        }
    }
}