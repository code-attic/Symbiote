using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Symbiote.Jackalope;
using Symbiote.Log4Net;
using Symbiote.StructureMap;

namespace PublishDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Configure();
            Publish();
        }

        static void Configure()
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Jackalope(x => x.AddServer(s => s.AMQP091().Address("localhost")))
                .AddConsoleLogger<IBus>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddConsoleLogger<Publisher>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .Dependencies(x => x.For<Publisher>().Use<Publisher>());
        }

        static void Publish()
        {
            var publisher = ServiceLocator.Current.GetInstance<Publisher>();
            publisher.Start();
        }
    }
}
