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
using System.Text;
using RabbitMQ.Client;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;
using Symbiote.Core.Utility;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;

namespace Symbiote.Rabbit.Impl.Adapter
{
    public class RabbitQueueListener
        : QueueingBasicConsumer
    {
        protected IChannelProxy Proxy { get; set; }
        protected IDispatcher Dispatch { get; set; }
        protected RabbitEndpoint RabbitEndpoint { get; set; }
        protected IMessageSerializer Serializer { get; set; }
        protected bool Running { get; set; }
        protected RingBuffer RingBuffer { get; set; }

        public override void HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            byte[] body )
        {
            var envelope = GetEnvelope( properties );
            envelope.ByteStream = body;
            envelope.Initialize(
                consumerTag,
                properties,
                deliveryTag,
                exchange,
                Proxy,
                redelivered,
                routingKey
                );
            RingBuffer.Write( envelope );
        }

        public RabbitEnvelope GetEnvelope( IBasicProperties basicProperties )
        {
            var messageTypeBuffer = (byte[]) basicProperties.Headers["MessageType"];
            var messageTypeName = Encoding.UTF8.GetString( messageTypeBuffer );
            var messageType = Type.GetType( messageTypeName );
            var envelopeType = typeof( RabbitEnvelope<> ).MakeGenericType( messageType );
            return Activator.CreateInstance( envelopeType ) as RabbitEnvelope;
        }

        public object DispatchResult( object translatedEnvelope )
        {
            var envelope = translatedEnvelope as IEnvelope;
            if ( envelope != null )
            {
                Dispatch.Send( envelope );
            }
            else
            {
                HandlePoisonMessage( translatedEnvelope );
            }
            return envelope;
        }

        public object DeserializeMessage( object envelope )
        {
            try
            {
                var rabbitEnvelope = envelope as RabbitEnvelope;
                rabbitEnvelope.Message = Serializer.Deserialize( rabbitEnvelope.MessageType, rabbitEnvelope.ByteStream );
                if ( rabbitEnvelope.Message == null )
                    HandlePoisonMessage( envelope );
            }
            catch ( Exception e )
            {
                HandlePoisonMessage( envelope );
            }
            return envelope;
        }

        public void HandlePoisonMessage( object message )
        {
            var envelope = message as RabbitEnvelope;
            if ( envelope != null )
            {
                "Received bad message from \r\n\t Exchange: {0} \r\n\t Queue: {1} \r\n\t MessageId: {2} \r\n\t CorrelationId: {3} \r\n\t Type: {4}"
                    .ToInfo<RabbitQueueListener>(
                        envelope.Exchange,
                        Proxy.QueueName,
                        envelope.MessageId,
                        envelope.CorrelationId,
                        envelope.MessageType
                    );
            }
            else
            {
                "Received garbage from queue {0}. No deserialization was possible."
                    .ToInfo<RabbitQueueListener>( Proxy.QueueName );
            }
        }

        public override void HandleBasicCancelOk( string consumerTag )
        {
            Running = false;
        }

        public override void HandleModelShutdown( IModel model, ShutdownEventArgs reason )
        {
            Running = false;
        }

        public RabbitQueueListener( IChannelProxy proxy, IDispatcher dispatch, RabbitEndpoint endpoint )
        {
            Proxy = proxy;
            Dispatch = dispatch;
            RabbitEndpoint = endpoint;
            proxy.InitConsumer( this );
            Running = true;
            RingBuffer = new RingBuffer( 10000 );
            Serializer = Assimilate.GetInstanceOf( endpoint.SerializerType ) as IMessageSerializer;
            RingBuffer.AddTransform( DeserializeMessage );
            RingBuffer.AddTransform( DispatchResult );
            RingBuffer.Start();
        }
    }
}