using System;
using Symbiote.Messaging;

namespace Rabbit.Tests
{
    public class MessageHandler
        : IHandle<Actor, Message>
    {
        public Action<IEnvelope> Handle(Actor actor, Message message)
        {
            actor.Received(message.Id);
            //envelope.Acknowledge();
            //if (Actor.MessageIds.Count % 100 == 0)
            //{
            //    envelope.AcknowledgeAll();
            //}
            return x => x.Acknowledge();
        }
    }
}