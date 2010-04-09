using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StructureMap;

namespace Symbiote.Jackalope.Impl
{
    public class ActionDispatcher<TMessage> : IDispatch<TMessage>
        where TMessage : class
    {
        private Func<TMessage, IRespond, object> _processor;

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
                                       Respond = new Response(proxy, args)
                                   };
                _processor(envelope.Message as TMessage, envelope.Respond);
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
                    Respond = new Response(proxy, result)
                };
                _processor(envelope.Message as TMessage, envelope.Respond);
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
                return _processor(envelope.Message as TMessage, envelope.Respond);
            }
            catch (Exception e)
            {
                envelope.Respond.Reject();
                throw;
            }
        }

        public ActionDispatcher(Func<TMessage, IRespond, object> processor)
        {
            _processor = processor;
        }
    }
}