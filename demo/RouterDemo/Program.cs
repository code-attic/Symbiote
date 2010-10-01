using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Jackalope.Impl.Router;
using Symbiote.StructureMap;
using Symbiote.Daemon;
using Symbiote.Jackalope;

namespace RouterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Daemon(x => x
                    .Arguments(args)
                    .Name("messagerouter")
                    .Description("Message Routing Demo")
                    .DisplayName("Message Router"))
                .Jackalope(x => x
                    .AddServer(s => s.Defaults())
                    .AddServer(s => s.VirtualHost("control").Broker("control"))
                )
                .Dependencies(x => x.For<Router>().Use<Router>().AsSingleton())
                .RunDaemon();
        }
    }

    public class RoutingService
        : IDaemon
    {
        protected IBus Bus { get; set; }

        public void Start()
        {
            ConfigureEndpoints();
        }

        protected void ConfigureEndpoints()
        {
            Bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout));
            Bus.AddEndPoint(x => x.QueueName("subscriber"));
            Bus.BindQueue("subscriber", "publisher");
            Bus.AddEndPoint(
                x => x.Exchange("control", ExchangeType.fanout).QueueName("routing").RoutingKeys("subscriber").Broker("control"));
            Bus.DefineRouteFor<SubscriberOnline>(x => x.SendTo("control").WithRoutingKey(s => s.SourceQueue));
            Bus.Subscribe("routing");
        }

        public void Stop()
        {
            
        }

        public RoutingService(IBus bus)
        {
            Bus = bus;
        }
    }

    public class Router
        : BaseRouter
    {
        public Router(IBus bus, IRouteGroupManager routeGroupManager) : base(bus, routeGroupManager)
        {
        }
    }
}
