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
using Symbiote.Core.Extensions;
using Symbiote.Core.Impl.Futures;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Rabbit.Config;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class RabbitChannel :
        IChannel,
        IHaveChannelProxy
    {
        public string Name { get; set; }
        public IChannelProxy Proxy { get; set; }
        public IMessageSerializer Serializer { get; set; }
        public ChannelDefinition Definition { get; set; }
        public IDispatcher MessageDispatcher { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>( message, x => { } );
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId,
                ReplyToKey = ConfigureResponseChannel<TReply>()
            };

            modifyEnvelope(envelope);
            envelope.ByteStream = Serializer.Serialize(envelope.Message);
            var future = Future.Of<TReply>(() => Proxy.Send(envelope));
            MessageDispatcher.ExpectResponse<TReply>(envelope.MessageId.ToString(), future);
            return future;
        }

        public string ConfigureResponseChannel<TReply>()
        {
            var baseName = RabbitBroker.ResponseId;
            var messageType = typeof(TReply).Name;

            RabbitExtensions.AddRabbitChannel(null, x => x
                .AutoDelete()
                .Direct( baseName ));

            RabbitExtensions.AddRabbitQueue(null, x => x
                .AutoDelete()
                .ExchangeName( baseName )
                .QueueName("{0}.{1}.{2}".AsFormat(baseName, "response", messageType))
                .RoutingKeys(messageType)
                .NoAck()
                .StartSubscription());
            
            return messageType;
        }

        public void Send<TMessage>(TMessage message, Action<IEnvelope> modifyEnvelope)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            modifyEnvelope(envelope);
            envelope.ByteStream = Serializer.Serialize(envelope.Message);
            Proxy.Send(envelope);
        }

        public void Send<TMessage>(TMessage message)
        {
            Send( message, x => { } );
        }

        public RabbitChannel(IChannelProxy proxy, IMessageSerializer serializer, ChannelDefinition definition, IDispatcher dispatcher)
        {
            Definition = definition;
            Proxy = proxy;
            Serializer = serializer;
            MessageDispatcher = dispatcher;
        }
    }
}