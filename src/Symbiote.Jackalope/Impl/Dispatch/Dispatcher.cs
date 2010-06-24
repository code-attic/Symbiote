using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StructureMap;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;
using System.Linq;

namespace Symbiote.Jackalope.Impl
{
    public class Dispatcher<TMessage> : IDispatch<TMessage>
        where TMessage : class
    {
        protected List<Type> handlesMessagesOf { get; set; }

        public bool CanHandle(object payload)
        {
            return payload as TMessage != null;
        }

        public IEnumerable<Type> Handles
        {
            get
            {
                handlesMessagesOf = handlesMessagesOf ?? GetMessageChain().ToList();
                return handlesMessagesOf;
            }
        }

        private IEnumerable<Type> GetMessageChain()
        {
            yield return typeof (TMessage);
            var chain = Reflector.GetInheritenceChain(typeof (TMessage));
            if(chain != null)
            {
                foreach (var type in chain)
                {
                    yield return type;
                }
            }
            yield break;
        }

        public void Dispatch(Envelope envelope)
        {
            try
            {
                var handler = ObjectFactory.GetInstance<IMessageHandler<TMessage>>();
                handler.Process(envelope.Message as TMessage, envelope.MessageDelivery);
                //return envelope.Message;
            }
            catch (Exception e)
            {
                envelope.MessageDelivery.Reject();
                //throw;
            }
        }
    }
}