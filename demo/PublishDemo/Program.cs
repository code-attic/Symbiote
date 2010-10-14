using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Symbiote.Jackalope;
using Symbiote.Log4Net;
using Symbiote.StructureMap;
using Symbiote.Daemon;

namespace PublishDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Jackalope(x => x
                    .AddServer(s => s.AMQP091().Address("bootcamp3-pc"))
                    .AddServer(s => s.VirtualHost("control").Broker("control")))
                .Daemon(x => x
                    .Arguments(args)
                    .Name("DemoPublisher")
                    .DisplayName("Demo Publisher")
                    .Description("Publishes messages"))
                .AddConsoleLogger<IBus>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddConsoleLogger<Publisher>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .Dependencies(x => x.Scan(y =>
                {
                    y.TheCallingAssembly();
                    y.AddSingleImplementations();
                }))
                .RunDaemon();
        }
    }
}
