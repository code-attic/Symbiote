using System;
using System.Threading;
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
            _bus.AddEndPoint(x => x.Exchange("secondary", ExchangeType.fanout));
            _bus.AddEndPoint(x => x.QueueName("subscriber1"));
            _bus.AddEndPoint(x => x.QueueName("subscriber2"));
            _bus.BindQueue("subscriber1", "publisher");
            _bus.BindQueue("subscriber2", "secondary");

            _bus
                .QueueStreams["subscriber1"]
                .Do(x =>
                    x.MessageDelivery.Acknowledge())
                .BufferWithTime(TimeSpan.FromSeconds(1))
                .Subscribe(x =>
                               {
                                   "Processed {0} queue 1 messages in 1 second".ToInfo<Subscriber>(x.Count);
                               });

            _bus
                .QueueStreams["subscriber2"]
                .Do(x =>
                    x.MessageDelivery.Acknowledge())
                .BufferWithTime(TimeSpan.FromSeconds(1))
                .Subscribe(x =>
                {
                    "Processed {0} queue 2 messages in 1 second".ToInfo<Subscriber>(x.Count);
                });
        }
    }
}