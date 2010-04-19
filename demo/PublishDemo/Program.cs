using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using Symbiote.Core;
using Symbiote.Jackalope;
using Symbiote.Log4Net;

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
                .Core()
                .Jackalope(x => x.AddServer(s => s.AMQP08().Address("localhost")))
                .AddConsoleLogger<IBus>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddConsoleLogger<Publisher>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .Dependencies(x => x.For<Publisher>().Use<Publisher>());
        }

        static void Publish()
        {
            var publisher = ObjectFactory.GetInstance<Publisher>();
            publisher.Start();
        }
    }
}
