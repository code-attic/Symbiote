using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StructureMap;

namespace Symbiote.Jackalope.Impl
{
    public class ActionDispatcher<TMessage> : IDispatch<TMessage>
        where TMessage : class
    {
        private Func<TMessage, IMessageDelivery, object> _processor;

        public bool CanHandle(object payload)
        {
            return payload as TMessage != null;
        }

        public void Dispatch(object payload, IChannelProxy proxy, BasicDeliverEventArgs args)
        {
            try
            {
                var envelope = new Envelope()
                                   {
                                       Message = payload,
                                       MessageDelivery = new MessageDelivery(proxy, args)
                                   };
                _processor(envelope.Message as TMessage, envelope.MessageDelivery);
            }
            catch (Exception e)
            {
                proxy.Reject(args.DeliveryTag, true);
                throw;
            }
        }

        public void Dispatch(object payload, IChannelProxy proxy, BasicGetResult result)
        {
            try
            {
                var envelope = new Envelope()
                {
                    Message = payload,
                    MessageDelivery = new MessageDelivery(proxy, result)
                };
                _processor(envelope.Message as TMessage, envelope.MessageDelivery);
            }
            catch (Exception e)
            {
                proxy.Reject(result.DeliveryTag, true);
                throw;
            }
        }

        public object Dispatch(Envelope envelope)
        {
            try
            {
                return _processor(envelope.Message as TMessage, envelope.MessageDelivery);
            }
            catch (Exception e)
            {
                envelope.MessageDelivery.Reject();
                throw;
            }
        }

        public ActionDispatcher(Func<TMessage, IMessageDelivery, object> processor)
        {
            _processor = processor;
        }
    }
}