using System;
using RabbitMQ.Client;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Serialization;
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
            var envelopeType = typeof(RabbitEnvelope<>).MakeGenericType(body.GetType());
            var envelope = Assimilate.GetInstanceOf(envelopeType) as RabbitEnvelope;

            envelope.ConsumerTag = consumerTag;
            envelope.DeliveryTag = deliveryTag;
            envelope.Exchange = exchange;
            envelope.Message = body;
            envelope.Proxy = proxy;
            envelope.Redelivered = redelivered;
            envelope.ReplyToExchange = properties.ReplyToAddress.ExchangeName;
            envelope.ReplyToKey = properties.ReplyToAddress.RoutingKey;
            envelope.RoutingKey = routingKey;
            envelope.TimeStamp = DateTime.FromFileTimeUtc(properties.Timestamp.UnixTime);

            return envelope;
        }
    }

    public class RabbitEnvelope<TMessage> :
        RabbitEnvelope,
        IEnvelope<TMessage>
        where TMessage : class
    {
        public new TMessage Message 
        {
            get { return base.Message as TMessage; }
            set { base.Message = value; }
        }
    }
}