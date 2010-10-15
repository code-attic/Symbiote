using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Rabbit.Config;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Rabbit;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;
using Symbiote.Messaging.Impl.Dispatch;
using Microsoft.Practices.ServiceLocation;

namespace Rabbit.Tests
{
    public class with_rabbit_configuration
    {
        protected static IBus Bus { get; set; }
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Messaging()
                                                .Rabbit(x => x.AddBroker(r => r.Defaults()));
                                            Bus = Assimilate.GetInstanceOf<IBus>();
                                        };
    }

    public class when_sending_messages
        : with_rabbit_configuration
    {
        private Because of = () =>
                                 {
                                     Bus.AddRabbitQueue("test", x => x.Direct("test").QueueName("test").NoAck());
                                     Bus.Send("test", new Message() {Id = 1, CorrelationId = "1"});
                                     Bus.Send("test", new Message() {Id = 2, CorrelationId = "1"});
                                     Bus.Send("test", new Message() {Id = 3, CorrelationId = "1"});

                                     Thread.Sleep(1000);
                                 };

        private It actor_should_have_received_three_messages = () => Actor.MessageIds.ShouldContain(1, 2, 3);
    }

    public class when_sending_400000_messages_to_60_actors
        : with_rabbit_configuration
    {
        protected static List<Actor> cast { get; set; }
        protected static Stopwatch watch { get; set; }
        protected static int MessagesToSend = 400000;
        protected static int actorCount = 60;
        private Because of = () =>
        {
            Actor.Created = 0;
            
            Bus.AddRabbitQueue("test", x => x.Direct("test").QueueName("test").NoAck());
            
            var names = Enumerable.Range(0, actorCount).Select(x => "Extra " + x).ToList();
            var message = Enumerable.Range(0, actorCount)
                .Select(x => new Message() { CorrelationId = names[x] })
                .ToList();

            watch = Stopwatch.StartNew();
            for (int i = 0; i < MessagesToSend; i++)
            {
                message[i % actorCount].Id = i;
                Bus.Send("test", message[i % actorCount]);
            }
            watch.Stop();

            cast = new List<Actor>();
            var manager = ServiceLocator.Current.GetInstance<IDispatcher>();
            var agency = ServiceLocator.Current.GetInstance<IAgency>();
            var agent = agency.GetAgentFor<Actor>();
            for (int i = 0; i < actorCount; i++)
            {
                var actor = agent.GetActor("Extra " + i);
                cast.Add(actor);
            }

            //Thread.Sleep(TimeSpan.FromSeconds(2));
        };

        private It should_complete_in_1_second = () =>
                                                 watch.ElapsedMilliseconds.ShouldBeLessThan(1001);

        private It should_only_have_created_the_actor_60_times = () =>
                                                                  Actor.Created.ShouldEqual(actorCount);

        private It should_have_sent_all_messages_to_actor = () =>
                                                            Actor.MessageIds.Count.ShouldEqual(MessagesToSend);
    }

    public class Message
        : ICorrelate
    {
        public int Id {get;set; }
        public string CorrelationId { get; set; }
    }

    public class Actor
    {
        public static int Created { get; set; }
        public static List<Actor> ArmyOfMehself = new List<Actor>();
        protected static List<int> _messages = new List<int>();
        public static List<int> MessageIds
        {
            get { return _messages; }
        }

        public void Received(int messageid)
        {
            _messages.Add(messageid);
        }

        public Actor()
        {
            Created++;
            ArmyOfMehself.Add(this);
        }
    }

    public class MessageHandler
        : IHandle<Actor, Message>
    {
        public void Handle(Actor actor, IEnvelope<Message> envelope)
        {
            actor.Received(envelope.Message.Id);
        }
    }
}
