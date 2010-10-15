using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Rabbit.Config;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Rabbit;

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

    public class Message
        : ICorrelate
    {
        public int Id {get;set; }
        public string CorrelationId { get; set; }
    }

    public class Actor
    {
        protected static List<int> _messages = new List<int>();
        public static List<int> MessageIds
        {
            get { return _messages; }
        }

        public void Received(int messageid)
        {
            _messages.Add(messageid);
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
