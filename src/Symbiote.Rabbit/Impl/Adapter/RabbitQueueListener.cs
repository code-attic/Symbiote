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
using System.Text;
using RabbitMQ.Client;
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
        protected int TotalReceived { get; set; }
        protected bool Running { get; set; }
        protected VolatileRingBuffer RingBuffer { get; set; }

        public override void HandleBasicDeliver(
            string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            byte[] body)
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
            RingBuffer.Write(envelope);
        }

        public RabbitEnvelope GetEnvelope(IBasicProperties basicProperties)
        {
            var envelopeType = typeof(RabbitEnvelope<>).MakeGenericType(
                Type.GetType(
                    Encoding.UTF8.GetString( (byte[]) basicProperties.Headers["MessageType"] )
                    ) );
            return Activator.CreateInstance( envelopeType ) as RabbitEnvelope;
        }

        public object DispatchResult(object translatedEnvelope)
        {
            Dispatch.Send(translatedEnvelope as IEnvelope);
            return null;
        }

        public object DeserializeMessage(object envelope)
        {
            var rabbitEnvelope = envelope as RabbitEnvelope;
            rabbitEnvelope.Message = Serializer.Deserialize( rabbitEnvelope.MessageType, rabbitEnvelope.ByteStream );
            return rabbitEnvelope;
        }

        public override void HandleBasicCancelOk(string consumerTag)
        {
            Running = false;
        }

        public override void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            Running = false;
        }

        public RabbitQueueListener(IChannelProxy proxy, IDispatcher dispatch, RabbitEndpoint endpoint)
        {
            Proxy = proxy;
            Dispatch = dispatch;
            RabbitEndpoint = endpoint;
            proxy.InitConsumer(this);
            Running = true;
            RingBuffer = new VolatileRingBuffer(100000);

            RingBuffer.AddTransform(DeserializeMessage);
            RingBuffer.AddTransform(DispatchResult);
            RingBuffer.Start();
        }
    }

    public class RabbitQueueListener<TMessage>
        : QueueingBasicConsumer
    {
        protected IChannelProxy Proxy { get; set; }
        protected IDispatcher Dispatch { get; set; }
        protected RabbitEndpoint RabbitEndpoint { get; set; }
        protected IMessageSerializer Serializer { get; set; }
        protected int TotalReceived { get; set; }
        protected bool Running { get; set; }
        protected VolatileRingBuffer RingBuffer { get; set; }

        public override void HandleBasicDeliver(
            string consumerTag, 
            ulong deliveryTag, 
            bool redelivered, 
            string exchange, 
            string routingKey, 
            IBasicProperties properties, 
            byte[] body)
        {
            var envelope = 
                    RabbitEnvelope<TMessage>.Create(
                        Proxy,
                        consumerTag,
                        deliveryTag,
                        redelivered,
                        exchange,
                        routingKey,
                        properties,
                        body);

            RingBuffer.Write( envelope );
        }

        public object DispatchResult(object translatedEnvelope)
        {
            Dispatch.Send( translatedEnvelope as IEnvelope<TMessage> );
            return null;
        }

        public object DeserializeMessage(object envelope)
        {
            var rabbitEnvelope = envelope as RabbitEnvelope;
            rabbitEnvelope.Message = Serializer.Deserialize<TMessage>(rabbitEnvelope.ByteStream);
            return rabbitEnvelope;
        }

        public override void HandleBasicCancelOk(string consumerTag)
        {
            Running = false;
        }

        public override void HandleModelShutdown(IModel model, ShutdownEventArgs reason)
        {
            Running = false;
        }

        public RabbitQueueListener(IChannelProxy proxy, IDispatcher dispatch, RabbitEndpoint endpoint)
        {
            Proxy = proxy;
            Dispatch = dispatch;
            RabbitEndpoint = endpoint;
            proxy.InitConsumer(this);
            Running = true;
            RingBuffer = new VolatileRingBuffer( 1000000 );
            
            RingBuffer.AddTransform( DeserializeMessage );
            RingBuffer.AddTransform( DispatchResult );
            RingBuffer.Start();
        }
    }
}