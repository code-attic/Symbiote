using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StructureMap;

namespace Symbiote.Jackalope.Impl
{
    public class ActionDispatcher<TMessage> : IDispatch<TMessage>
        where TMessage : class
    {
        private Action<TMessage, IResponse> _messageHandler;

        public bool CanHandle(object payload)
        {
            return payload as TMessage != null;
        }

        public void Dispatch(object payload, IChannelProxy proxy, BasicDeliverEventArgs args)
        {
            try
            {
                var response = new Response(proxy, args);
                _messageHandler(payload as TMessage, response);
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
                var response = new Response(proxy, result);
                _messageHandler(payload as TMessage, response);
            }
            catch (Exception e)
            {
                proxy.Reject(result.DeliveryTag, true);
                throw;
            }
        }

        public ActionDispatcher(Action<TMessage, IResponse> messageHandler)
        {
            _messageHandler = messageHandler;
        }
    }
}