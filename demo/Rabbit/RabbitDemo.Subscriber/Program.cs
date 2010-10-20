using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RabbitDemo.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Daemon.Host;
using Symbiote.Log4Net;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;
using Symbiote.Rabbit;
using Symbiote.StructureMap;

namespace RabbitDemo.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Messaging()
                .Rabbit(x => x.AddBroker(r => r.Defaults().Address("bootcamp2-pc")))
                .AddConsoleLogger<Subscriber>(x => x.Info().MessageLayout(m => m.TimeStamp().Message().Newline()))
                .AddConsoleLogger<IHost>(x => x.Info().MessageLayout(m => m.TimeStamp().Message().Newline()))
                .Daemon(x => x.Name("subscriber").Arguments(args))
                .RunDaemon();
        }
    }

    public class Subscriber
        : IDaemon
    {
        public IBus Bus { get; set; }
        public int MessageCount { get; set; }
        public int ActorCount { get; set; }

        public void Start()
        {
            try
            {
                "Starting Subscriber".ToInfo<Subscriber>();
                "Configuring Rabbit...".ToInfo<Subscriber>();
                Bus.AddRabbitQueue("test", x => x.Fanout("test").QueueName("test").NoAck());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Stop()
        {

        }

        public Subscriber(IBus bus)
        {
            Bus = bus;
        }
    }

    public class ForActorMessage
        : IHandle<Actor, Message>
    {
        public void Handle(Actor actor, IEnvelope<Message> envelope)
        {
            actor.AddMessageId(envelope.Message.MessageId);
        }
    }

    public class Actor
    {
        public string Name { get; set; }
        public List<int> Ids { get; set; }

        public void AddMessageId(int Id)
        {
            Ids.Add(Id);
            if(Ids.Count % 100 == 0)
                "Actor {0} has {1} messages!"
                .ToInfo<Subscriber>(Name, Ids.Count);
        }

        public Actor(string name)
        {
            Ids = new List<int>();
            Name = name;

            "New Actor, {0}, created!"
                .ToInfo<Subscriber>(name);
        }
    }

    public class ActorFactory
        : IActorFactory<Actor>
    {
        public Actor CreateInstance<TKey>(TKey id)
        {
            return new Actor(id.ToString());
        }
    }
}
