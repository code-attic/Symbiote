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
    public class Subscriber : IDaemon
    {
        private IBus Bus;

        public void Start()
        {
            "Subscriber is starting.".ToInfo<Subscriber>();
            Bus.Subscribe("subscriber1");
            Bus.Subscribe("subscriber2");
            Bus.Subscribe("subscriber3");
            //Bus.Subscribe("subscriber4");
            //Bus.Subscribe("subscriber5");
        }

        public void Stop()
        {
            "Stop was called."
                .ToInfo<Subscriber>();
        }

        public Subscriber(IBus bus)
        {
            Bus = bus;
            Bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout));
            //Bus.AddEndPoint(x => x.QueueName("subscriber").LoadBalanced());
            //Bus.AddEndPoint(x => x.QueueName("subscriber"));
            //Bus.BindQueue("subscriber", "publisher");
            //Bus.AddEndPoint(
            //    x => x.Exchange("control", ExchangeType.fanout).QueueName("routing").RoutingKeys("subscriber").Broker("control"));
            //Bus.DefineRouteFor<SubscriberOnline>(x => x.SendTo("control").WithRoutingKey(s => s.SourceQueue));
            Bus.AddEndPoint(x => x.QueueName("subscriber1"));
            Bus.AddEndPoint(x => x.QueueName("subscriber2"));
            Bus.AddEndPoint(x => x.QueueName("subscriber3"));
            //Bus.AddEndPoint(x => x.QueueName("subscriber4"));
            //Bus.AddEndPoint(x => x.QueueName("subscriber5"));
            Bus.BindQueue("subscriber1", "publisher");
            Bus.BindQueue("subscriber2", "publisher");
            Bus.BindQueue("subscriber3", "publisher");
            //Bus.BindQueue("subscriber4", "publisher");
            //Bus.BindQueue("subscriber5", "publisher");
        }
    }
}