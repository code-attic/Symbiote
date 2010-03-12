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
        public bool CanHandle(object payload)
        {
            return payload as TMessage != null;
        }

        public void Dispatch(object payload, IChannelProxy proxy, BasicDeliverEventArgs args)
        {
            try
            {
                var handler = ObjectFactory.GetInstance<IMessageHandler<TMessage>>();
                var response = new Response(proxy, args);
                handler.Process(payload as TMessage, response);
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
                var handler = ObjectFactory.GetInstance<IMessageHandler<TMessage>>();
                var response = new Response(proxy, result);
                handler.Process(payload as TMessage, response);
            }
            catch (Exception e)
            {
                proxy.Reject(result.DeliveryTag, true);
                throw;
            }
        }
    }
}