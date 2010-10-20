using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging;

namespace Symbiote.Rabbit
{
    public abstract class RabbitMessageHandler<TMessage> :
        IHandle<TMessage>
        where TMessage : class
    {
        public void Handle(IEnvelope<TMessage> envelope)
        {
            Handle(envelope as RabbitEnvelope<TMessage>);
        }

        public abstract void Handle(RabbitEnvelope<TMessage> envelope);
    }

    public abstract class RabbitActorHandler<TActor, TMessage> :
        IHandle<TActor, TMessage>
        where TMessage : class, ICorrelate
        where TActor : class
    {
        public void Handle(TActor actor, IEnvelope<TMessage> envelope)
        {
            Handle(actor, envelope as RabbitEnvelope<TMessage>);
        }

        public abstract void Handle(TActor actor, RabbitEnvelope<TMessage> envelope);
    }
}
