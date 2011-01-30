// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using RabbitMQ.Client;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Rabbit.Impl.Channels;

namespace Symbiote.Rabbit
{
    public class RabbitEnvelope
    {
        protected static readonly long EPOCH = 621355968000000000L;
        protected string _correlationId;
        protected Type _messageType;
        public byte[] ByteStream { get; set; }
        public string ConsumerTag { get; set; }
        public string CorrelationId { get; set; }
        public ulong DeliveryTag { get; set; }
        public string Exchange { get; set; }
        public Guid MessageId { get; set; }

        public IChannelProxy Proxy { get; set; }
        public bool Redelivered { get; set; }
        public string ReplyToExchange { get; set; }
        public string ReplyToKey { get; set; }
        public string RoutingKey { get; set; }
        public DateTime TimeStamp { get; set; }

        public long Sequence { get; set; }
        public long Position { get; set; }
        public bool SequenceEnd { get; set; }

        public virtual object Message { get; set; }

        public bool Empty
        {
            get { return Message == null; }
        }

        public Type MessageType
        {
            get
            {
                _messageType = _messageType ?? Message.GetType();
                return _messageType;
            }
            protected set { _messageType = value; }
        }

        public void Initialize( string consumerTag, IBasicProperties properties, ulong deliveryTag, string exchange,
                                IChannelProxy proxy, bool redelivered, string routingKey )
        {
            ConsumerTag = consumerTag;
            CorrelationId = properties.CorrelationId;
            DeliveryTag = deliveryTag;
            Exchange = exchange;
            MessageId = Guid.Parse( properties.MessageId );
            Proxy = proxy;
            Redelivered = redelivered;
            ReplyToExchange = properties.ReplyToAddress.ExchangeName;
            ReplyToKey = properties.ReplyToAddress.RoutingKey;
            RoutingKey = routingKey;
            Position = (long) properties.Headers["Position"];
            Sequence = (long) properties.Headers["Sequence"];
            SequenceEnd = (bool) properties.Headers["SequenceEnd"];
            TimeStamp = properties.Timestamp.UnixTime.FromUnixTimestamp();
        }

        public RabbitEnvelope()
        {
        }

        public RabbitEnvelope( string consumerTag, IBasicProperties properties, ulong deliveryTag, string exchange,
                               IChannelProxy proxy, bool redelivered, string routingKey, byte[] body )
        {
            ByteStream = body;
            Initialize( consumerTag, properties, deliveryTag, exchange, proxy, redelivered, routingKey );
        }
    }

    public class RabbitEnvelope<TMessage> :
        RabbitEnvelope,
        IEnvelope<TMessage>
    {
        public new TMessage Message
        {
            get { return (TMessage) base.Message; }
            set { base.Message = value; }
        }

        public void Reply<TResponse>( TResponse response )
        {
            var bus = Assimilate.GetInstanceOf<IBus>();
            if ( !bus.HasChannelFor( ReplyToExchange ) )
            {
                bus.AddRabbitChannel( x => x.AutoDelete().Direct( ReplyToExchange ) );
            }
            bus.Publish( ReplyToExchange, response, x =>
                                                        {
                                                            x.CorrelationId = MessageId.ToString();
                                                            x.RoutingKey = ReplyToKey;
                                                        } );
        }

        public void Acknowledge()
        {
            AckMessage( false );
        }

        public void AcknowledgeAll()
        {
            AckMessage( true );
        }

        protected void AckMessage( bool all )
        {
            Proxy.Acknowledge( DeliveryTag, all );
        }

        public void Reject()
        {
            Proxy.Reject( DeliveryTag, true );
        }

        public static RabbitEnvelope<TMessage> Create( IChannelProxy proxy, string consumerTag, ulong deliveryTag,
                                                       bool redelivered, string exchange, string routingKey,
                                                       IBasicProperties properties, TMessage body )
        {
            var envelope = new RabbitEnvelope<TMessage>( body );
            envelope.Initialize( consumerTag, properties, deliveryTag, exchange, proxy, redelivered, routingKey );
            return envelope;
        }

        public static RabbitEnvelope<TMessage> Create( IChannelProxy proxy, string consumerTag, ulong deliveryTag,
                                                       bool redelivered, string exchange, string routingKey,
                                                       IBasicProperties properties, byte[] body )
        {
            var envelope = new RabbitEnvelope<TMessage>();
            envelope.ByteStream = body;
            envelope.Initialize( consumerTag, properties, deliveryTag, exchange, proxy, redelivered, routingKey );
            return envelope;
        }

        public RabbitEnvelope()
        {
            MessageId = Guid.NewGuid();
            _messageType = typeof( TMessage );
        }

        public RabbitEnvelope( TMessage message ) : this()
        {
            Message = message;
        }
    }
}