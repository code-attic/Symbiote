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
using Symbiote.Messaging;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit
{
    public class RabbitEnvelope<TMessage> :
        IEnvelope<TMessage>
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

        public long Sequence { get; set; }
        public long Position { get; set; }
        public bool SequenceEnd { get; set; }

        public void Acknowledge()
        {
            AckMessage(false);
        }

        public void AcknowledgeAll()
        {
            AckMessage(true);
        }

        protected void AckMessage(bool all)
        {
            Proxy.Acknowledge(DeliveryTag, all);
        }

        public void Reject()
        {
            Proxy.Reject(DeliveryTag, true);
        }

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