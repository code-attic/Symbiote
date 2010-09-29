using System;
using System.Diagnostics;
using System.Threading;
using Demo.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Jackalope;
using System.Linq;
using Symbiote.Jackalope.Impl.Router;
using Symbiote.Log4Net;
using Symbiote.StructureMap;

namespace SubscribeDemo
{
    public class Subscriber : BootstrappedDaemon<Subscriber>
    {
        private IBus Bus;

        public override void Start()
        {
            "Subscriber is starting.".ToInfo<Subscriber>();
            var latencyTotal1 = 0d;
            var latencyTotal2 = 0d;
            var count1 = 0d;
            var count2 = 0d;
            var watch = new Stopwatch();
            Bus
                .QueueStreams["subscriber"]
                .Do(x =>
                        {
                            if(count1 == 0)
                                watch.Start();
                            x.MessageDelivery.Acknowledge();
                            count1++;
                            latencyTotal1 += DateTime.Now.Subtract((x.Message as Message).Created).TotalMilliseconds;
                        })
                .BufferWithTime(TimeSpan.FromSeconds(1))
                .Subscribe(x =>
                {
                    var avgLatency = latencyTotal1 / (count1 == 0 ? 1 : count1);
                    var msgsPerSecond = count1 / watch.Elapsed.TotalSeconds;
                    "Q1: {0} msgs total. {1} msgs/sec. {2} ms latency / msg."
                        .ToInfo<Subscriber>(count1, msgsPerSecond, avgLatency);
                    Thread.Sleep(TimeSpan.FromMilliseconds(93));
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
            Bus = bus;
            Bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout));
            Bus.AddEndPoint(x => x.QueueName("subscriber").LoadBalanced());
            Bus.BindQueue("subscriber", "publisher");
            Bus.AddEndPoint(
                x => x.Exchange("control", ExchangeType.fanout).QueueName("routing").RoutingKeys("subscriber").Broker("control"));
            Bus.DefineRouteFor<SubscriberOnline>(x => x.SendTo("control").WithRoutingKey(s => s.SourceQueue));
            Bus.Subscribe("subscriber");
        }
    }
}