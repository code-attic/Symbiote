using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using StructureMap;
using Symbiote.Core;
using Symbiote.Core.Extensions;
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
                .Jackalope(x => x.AddServer(s => s.Address("localhost").AMQP08()))
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

    public class Message
    {
        public virtual string Body { get; set; }

        public Message(string body)
        {
            Body = body;
        }
    }

    public class Publisher
    {
        private IBus _bus;

        public void Start()
        {
            "Publisher is starting.".ToInfo<Publisher>();
            long i = 0;
            while(true)
            {
                _bus.Send("publisher", new Message("Message {0}".AsFormat(++i)));
                Thread.Sleep(5);
            }
        }

        public Publisher(IBus bus)
        {
            _bus = bus;
            _bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout).Durable().PersistentDelivery());
        }
    }
}
