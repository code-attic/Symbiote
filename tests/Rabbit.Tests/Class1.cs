using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Core;
using Symbiote.Core.Impl.UnitOfWork;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Rabbit;
using Symbiote.Messaging.Impl.Dispatch;

namespace Rabbit.Tests
{
    public class with_rabbit_configuration
    {
        protected static IBus Bus { get; set; }
        private Establish context = () =>
                                        {
                                            Assimilate
                                                .Core<StructureMapAdapter>()
                                                .Actors()
                                                .Messaging()
                                                .Rabbit(x => x.AddBroker(r => r.Defaults().Address("localhost")));
                                            Bus = Assimilate.GetInstanceOf<IBus>();
                                        };
    }

    public class when_sending_messages
        : with_rabbit_configuration
    {
        private Because of = () =>
        {
                                     Bus.AddRabbitChannel( x => x.Direct( "test" ).CorrelateBy<Message>( m => m.CorrelationId ) );
                                     Bus.AddRabbitQueue(x => x.ExchangeName("test").QueueName("test").NoAck().StartSubscription());
                                        
                                     Bus.Publish("test", new Message() {Id = 1, CorrelationId = "1"});
                                     Bus.Publish("test", new Message() { Id = 2, CorrelationId = "1" });
                                     Bus.Publish("test", new Message() { Id = 3, CorrelationId = "1" });

                                     Thread.Sleep(40);
                                 };
        
        private It actor_should_have_received_three_messages = () => Actor.MessageIds.ShouldContain(1, 2, 3);
    }

    public class when_sending_400000_messages_to_60_actors
        : with_rabbit_configuration
    {
        protected static List<Actor> cast { get; set; }
        protected static Stopwatch receiveWatch { get; set; }
        protected static Stopwatch sendWatch { get; set; }
        protected static int MessagesToSend = 10000;
        protected static int actorCount = 60;
        protected static IDispatcher dispatcher;

        private Because of = () =>
        {
            Actor.Created = 0;

            Bus.AddRabbitChannel(x => x.Direct("test").CorrelateBy<Message>(m => m.CorrelationId).AutoDelete());
            Bus.AddRabbitQueue(x => x.ExchangeName("test").QueueName("test").AutoDelete().NoAck());
            
            var names = Enumerable.Range(0, actorCount).Select(x => "Extra " + x).ToList();
            var message = Enumerable.Range(0, actorCount)
                .Select(x => new Message() { CorrelationId = names[x] })
                .ToList();

            
            sendWatch = Stopwatch.StartNew();
            for (int i = 0; i < MessagesToSend; i++)
            {
                message[i % actorCount].Id = i;
                Bus.Publish("test", message[i % actorCount]);
                //if(i % 1000 == 0)
                //    Bus.CommitChannelOf<Message>();
            }
            sendWatch.Stop();

            Bus.StartSubscription("test");
            receiveWatch = Stopwatch.StartNew();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            receiveWatch.Stop(); 

            dispatcher = Assimilate.GetInstanceOf<IDispatcher>();
        };
        
        private It should_receive_in_1_second = () =>
                                                 receiveWatch.ElapsedMilliseconds.ShouldBeLessThan(10);

        private It should_send_in_1_second = () =>
                                                 sendWatch.ElapsedMilliseconds.ShouldBeLessThan(10);

        private It should_only_have_created_the_actor_60_times = () =>
                                                                  Actor.Created.ShouldEqual(actorCount);

        private It should_have_sent_all_messages_to_actor = () =>
                                                            Actor.MessageIds.Count.ShouldEqual(MessagesToSend);
        
        private It should_have_all_teh_dispatchers = () => dispatcher.Count.ShouldEqual(MessagesToSend);
    }

    [Serializable]
    [DataContract]
    public class Message
    {
        [DataMember(Order = 1)]
        public int Id { get;set; }
        [DataMember(Order = 2)]
        public string CorrelationId { get; set; }
    }

    public class Actor
    {
        public string Id { get; set; }
        public static int Created { get; set; }
        public static List<Actor> ArmyOfMehself = new List<Actor>();
        protected static List<int> _messages = new List<int>();
        public static List<int> MessageIds
        {
            get { return _messages; }
        }

        public void Received(int messageid)
        {
            if(_messages.Contains( messageid ))
                throw new Exception("This is crap!");
            _messages.Add(messageid);
        }

        public Actor()
        {
            Created++;
            ArmyOfMehself.Add(this);
        }
    }

    public class MessageHandler
        : RabbitActorHandler<Actor, Message>
    {
        public override void Handle(Actor actor, RabbitEnvelope<Message> envelope)
        {
            actor.Received(envelope.Message.Id);
            //envelope.Acknowledge();
            //if (Actor.MessageIds.Count % 100 == 0)
            //{
            //    envelope.AcknowledgeAll();
            //}
        }
    }

    public class ActorKeyAccessor : IKeyAccessor<Actor>
    {
        public string GetId( Actor actor )
        {
            return actor.Id;
        }

        public void SetId<TKey>( Actor actor, TKey id )
        {
            actor.Id = id.ToString();
        }
    }
}
