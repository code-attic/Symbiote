using System;
using System.Threading;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Jackalope;
using System.Linq;
using Symbiote.Log4Net;
using Symbiote.StructureMap;

namespace SubscribeDemo
{
    public class Subscriber : BootstrappedDaemon<Subscriber>
    {
        private IBus _bus;

        public override void Start()
        {
            "Subscriber is starting.".ToInfo<Subscriber>();

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

        public override void Stop()
        {
            "Stop was called."
                .ToInfo<Subscriber>();
        }

        public override void Initialize()
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .AddColorConsoleLogger<IBus>(x => x
                        .Info()
                        .MessageLayout(m => m.Message().Newline())
                        .DefineColor()
                        .Text.IsHighIntensity().BackGround.IsRed().ForAllOutput())
                .AddConsoleLogger<Subscriber>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddFileLogger<Subscriber>(x => x
                        .Debug()
                        .MessageLayout(m => m.Message().Newline())
                        .FileName(@"C:\git\Symbiote\demo\TopShelfHost\Services\SubscribeDemo\subscriber.log"))
                .Jackalope(x => x.AddServer(s => s.AMQP091().Address("localhost")));
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
        }
    }
}