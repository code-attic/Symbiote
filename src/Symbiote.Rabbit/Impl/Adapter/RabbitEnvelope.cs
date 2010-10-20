/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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
            //var envelope = Assimilate.GetInstanceOf(envelopeType) as RabbitEnvelope;
            var envelope = Activator.CreateInstance(envelopeType) as RabbitEnvelope;

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
        //RabbitEnvelope,
        IEnvelope<TMessage>
        where TMessage : class
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
        public TMessage Message { get; set; }

        public static RabbitEnvelope<TMessage> Create(IChannelProxy proxy, string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, TMessage body)
        {
            var envelope = new RabbitEnvelope<TMessage>();

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
}