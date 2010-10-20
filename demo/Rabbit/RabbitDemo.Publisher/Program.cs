using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RabbitDemo.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Rabbit;
using Symbiote.Log4Net;
using Symbiote.Daemon;

namespace RabbitDemo.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Messaging()
                .Rabbit(x => x.AddBroker(r => r.Defaults().Address("bootcamp2-pc")))
                .AddConsoleLogger<Publisher>(x => x.Info().MessageLayout(m => m.TimeStamp().Message().Newline()))
                .Daemon(x => x.Name("publisher").Arguments(args))
                .RunDaemon();
        }
    }

    public class Publisher
        : IDaemon
    {
        public IBus Bus { get; set; }
        public int MessageCount { get; set; }
        public int ActorCount { get; set; }

        public void Start()
        {
            "Starting Publisher".ToInfo<Publisher>();
            "Configuring Rabbit...".ToInfo<Publisher>();
            Bus.AddRabbitChannel("test", x => x.Fanout("test").QueueName("test").NoAck());

            "Creating {0} messages for {1} actors...".ToInfo<Publisher>(MessageCount, ActorCount);
            var messages = Enumerable.Range(1, MessageCount)
                .Select(x => new Message((x%ActorCount).ToString(), x))
                .ToList();

            "Sending Messages".ToInfo<Publisher>();
            var watch = Stopwatch.StartNew();
            messages
                .AsParallel()
                .ForAll(x => Bus.Send("test", x));
            watch.Stop();
            "{0} messages sent in {1} seconds"
                .ToInfo<Publisher>(MessageCount, watch.Elapsed.TotalSeconds);
        }

        public void Stop()
        {
            
        }

        public Publisher(IBus bus)
        {
            Bus = bus;
            MessageCount = 100000;
            ActorCount = 100;
        }
    }
}
