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
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Rabbit.Config;
using Symbiote.Rabbit.Impl.Endpoint;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class RabbitChannel<TMessage>
        : IChannel<TMessage>
    {
        public string Name { get; set; }
        public IChannelProxy Proxy { get; set; }
        public RabbitChannelDefinition<TMessage> Definition { get; set; }

        public void ExpectReply<TReply>(TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId,
                ReplyToKey = ConfigureResponseChannel<TReply>()
            };

            modifyEnvelope( envelope );
            dispatcher.ExpectResponse(envelope.MessageId.ToString(), onReply);
            var stream = Definition.OutgoingTransform.Transform<TMessage, byte[]>(message);
            Proxy.Send(envelope, stream);
        }

        public string ConfigureResponseChannel<TReply>()
        {
            var endpoints = Assimilate.GetInstanceOf<IEndpointManager>();
            var baseName = RabbitBroker.ResponseId;
            var messageType = typeof(TReply).Name;
            Action<RabbitEndpointFluentConfigurator<TReply>> configurate = x => x
                .AutoDelete()
                .Direct(baseName)
                .QueueName("{0}.{1}.{2}".AsFormat(baseName, "response", messageType))
                .RoutingKeys(messageType)
                .NoAck()
                .StartSubscription();
            endpoints.ConfigureEndpoint(configurate);
            return messageType;
        }

        public IEnvelope<TMessage> Send(TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            modifyEnvelope( envelope );
            var stream = Definition.OutgoingTransform.Transform<TMessage, byte[]>(message);
            Proxy.Send(envelope, stream);
            return envelope;
        }

        public IEnvelope<TMessage> Send(TMessage message)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            var stream = Definition.OutgoingTransform.Transform<TMessage, byte[]>(message);
            Proxy.Send(envelope, stream);
            return envelope;
        }

        public RabbitChannel(IChannelProxy proxy, RabbitChannelDefinition<TMessage> definition)
        {
            Definition = definition;
            Proxy = proxy;
        }
    }
}