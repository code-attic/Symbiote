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
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit
{
    public class RabbitEnvelope<TMessage> :
        IEnvelope<TMessage>
    {
        private static readonly long EPOCH = 621355968000000000L;
        protected Type _messageType;
        protected string _correlationId;
        public string ConsumerTag { get; set; }
        public string CorrelationId { get; set; }
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

        public void Reply<TResponse>( TResponse response )
        {
            var bus = Assimilate.GetInstanceOf<IBus>();
            if (!bus.HasChannelFor<TResponse>())
            {
                bus.AddRabbitChannel<TResponse>( x => x.AutoDelete().Direct( ReplyToExchange ).NoAck() );
            }
            bus.Publish(response, x =>
            {
                x.CorrelationId = MessageId.ToString();
                x.RoutingKey = ReplyToKey;
            } );
        }

        public TMessage Message { get; set; }

        public RabbitEnvelope( TMessage message )
        {
            Message = message;
            _messageType = typeof(TMessage);
            MessageId = Guid.NewGuid();
        }

        public static RabbitEnvelope<TMessage> Create(IChannelProxy proxy, string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, TMessage body)
        {
            var envelope = new RabbitEnvelope<TMessage>(body);

            envelope.ConsumerTag = consumerTag;
            envelope.CorrelationId = properties.CorrelationId;
            envelope.DeliveryTag = deliveryTag;
            envelope.Exchange = exchange;
            envelope.MessageId = Guid.Parse( properties.MessageId );
            envelope.Proxy = proxy;
            envelope.Redelivered = redelivered;
            envelope.ReplyToExchange = properties.ReplyToAddress.ExchangeName;
            envelope.ReplyToKey = properties.ReplyToAddress.RoutingKey;
            envelope.RoutingKey = routingKey;
            envelope.Position = (long) properties.Headers["Position"];
            envelope.Sequence = (long) properties.Headers["Sequence"];
            envelope.SequenceEnd = (bool) properties.Headers["SequenceEnd"];
            envelope.TimeStamp = new DateTime(EPOCH).AddTicks(properties.Timestamp.UnixTime);

            return envelope;
        }
    }
}