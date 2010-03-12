using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StructureMap;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class Response : IResponse, IDisposable
    {
        private IChannelProxy _proxy;
        private IChannelProxyFactory _factory;
        private string _exchange = "";
        private ulong _deliveryTag;
        private bool _isReplyToPresent;
        private IBasicProperties _properties;

        public void Acknowledge()
        {
            Respond(x => x.Acknowledge(_deliveryTag, false), _exchange);
        }

        public void Reject()
        {
            Respond(x => x.Reject(_deliveryTag, true), _exchange);
        }

        public void Reply<TReply>(TReply reply)
            where TReply : class
        {
            if(!_properties.IsReplyToPresent())
                return;

            Respond(x => x.Reply(_properties.ReplyToAddress, _properties, reply), _exchange);
        }

        private void Respond(Action<IChannelProxy> response, string exchange)
        {
            try
            {
                if(!_proxy.Channel.IsOpen)
                {
                    using(var proxy = _factory.GetProxyForQueue(exchange))
                    {
                        response(proxy);
                    }
                }
                else
                {
                    response(_proxy);   
                }
            }
            catch (Exception e)
            {
                "An exception occurred trying to respond to a received message: \r\n\t {0}"
                    .ToError<IBus>(e);
            }
        }

        public Response(IChannelProxy proxy, BasicDeliverEventArgs args)
        {
            _factory = ObjectFactory.GetInstance<IChannelProxyFactory>();
            _proxy = proxy;
            _exchange = args.Exchange;
            _deliveryTag = args.DeliveryTag;
            _properties = args.BasicProperties;
        }

        public Response(IChannelProxy proxy, BasicGetResult result)
        {
            _factory = ObjectFactory.GetInstance<IChannelProxyFactory>();
            _proxy = proxy;
            _exchange = result.Exchange;
            _deliveryTag = result.DeliveryTag;
            _properties = result.BasicProperties;
        }

        public void Dispose()
        {
            _proxy.Dispose();
        }
    }
}