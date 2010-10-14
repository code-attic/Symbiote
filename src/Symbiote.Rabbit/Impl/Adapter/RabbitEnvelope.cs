using System;
using RabbitMQ.Client;
using Symbiote.Messaging;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class RabbitEnvelope
        : IEnvelope
    {
        protected Type _messageType;
        protected string _correlationId;
        public string ConsumerTag { get; set; }
        public string CorrelationId
        {
            get
            {
                _correlationId = _correlationId ?? GetCorrelationFromMessage();
                return _correlationId;
            }
            set
            {
                _correlationId = value;
            }
        }
        public ulong DeliveryTag { get; set; }
        public bool Empty { get { return Message == null; } }
        public string Exchange { get; set; }
        public object Message { get; set; }
        public Guid MessageId { get; set; }
        public Type MessageType
        {
            get
            {
                _messageType = _messageType ?? Message.GetType();
                return _messageType;
            }
        }
        public IChannelProxy Proxy { get; set; }
        public bool Redelivered { get; set; }
        public string ReplyToExchange { get; set; }
        public string ReplyToKey { get; set; }
        public string RoutingKey { get; set; }
        public DateTime TimeStamp { get; set; }
        
        protected string GetCorrelationFromMessage()
        {
            var correlate = Message as ICorrelate;
            if (correlate == null)
                return "";
            return correlate.CorrelationId;
        }

        public RabbitEnvelope()
        {

        }

        public static RabbitEnvelope Create(IChannelProxy proxy, string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, object body)
        {
            return new RabbitEnvelope()
                       {
                           ConsumerTag = consumerTag,
                           DeliveryTag = deliveryTag,
                           Exchange = exchange,
                           Message = body,
                           Proxy = proxy,
                           Redelivered = redelivered,
                           ReplyToExchange = properties.ReplyToAddress.ExchangeName,
                           ReplyToKey = properties.ReplyToAddress.RoutingKey,
                           RoutingKey = routingKey,
                           TimeStamp = DateTime.FromFileTimeUtc(properties.Timestamp.UnixTime),
                       };
        }
    }
}