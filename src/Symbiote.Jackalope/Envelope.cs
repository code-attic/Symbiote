using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Symbiote.Jackalope.Impl;
using Symbiote.Jackalope.Impl.Channel;

namespace Symbiote.Jackalope
{
    public class Envelope : IDisposable
    {
        protected Type messageType { get; set; }
        public bool Empty { get { return MessageDelivery == null || Message == null; } }
        public IMessageDelivery MessageDelivery { get; set; }
        public object Message { get; set; }
        public Type MessageType
        {
            get
            {
                messageType = messageType ?? Message.GetType();
                return messageType;
            }
        }
        public string CorrelationId
        {
            get
            {
                var correlate = Message as ICorrelate;
                if (correlate == null)
                    return "";
                return correlate.CorrelationId;
            }
        }

        public Envelope()
        {
        }

        public static Envelope Create(object message, IChannelProxy proxy, BasicDeliverEventArgs args)
        {
            return new Envelope()
            {
                Message = message,
                MessageDelivery = new MessageDelivery(proxy, args)
            };
        }

        public static Envelope Create(object message, IChannelProxy proxy, BasicGetResult args)
        {
            return new Envelope()
                       {
                           Message = message,
                           MessageDelivery = new MessageDelivery(proxy, args)
                       };
        }

        public static Envelope Create(object message, IChannelProxy proxy, BasicReturnEventArgs args)
        {
            return new Envelope()
            {
                Message = message,
                MessageDelivery = new MessageDelivery(proxy, args)
            };
        }

        public static Envelope Create(IChannelProxy proxy, string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, object body)
        {
            return new Envelope()
                       {
                           Message = body,
                           MessageDelivery =
                               new MessageDelivery(proxy, consumerTag, deliveryTag, redelivered, exchange, routingKey,
                                                   properties)
                       };
        }

        public void Dispose()
        {
            (MessageDelivery as IDisposable).Dispose();
        }
    }
}