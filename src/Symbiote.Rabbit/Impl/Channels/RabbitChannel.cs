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
    public interface IHaveChannelProxy
    {
        IChannelProxy Proxy { get; set; }
    }

    public class RabbitChannel :
        IOpenChannel
        , IHaveChannelProxy
    {
        public string Name { get; set; }
        public IChannelProxy Proxy { get; set; }
        public RabbitChannelDefinition Definition { get; set; }

        public void ExpectReply<TMessage, TReply>(TMessage message, Action<IEnvelope> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId,
                ReplyToKey = ConfigureResponseChannel<TReply>()
            };

            modifyEnvelope(envelope);
            dispatcher.ExpectResponse(envelope.MessageId.ToString(), onReply);
            Definition.OutgoingTransform.Transform<RabbitEnvelope, RabbitEnvelope>(envelope);
            Proxy.Send(envelope);
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

        public void Send<TMessage>(TMessage message, Action<IEnvelope> modifyEnvelope)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            modifyEnvelope(envelope);
            Definition.OutgoingTransform.Transform<RabbitEnvelope, RabbitEnvelope>(envelope);
            Proxy.Send(envelope);
        }

        public void Send<TMessage>(TMessage message)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.GetCorrelationId(message),
                RoutingKey = Definition.GetRoutingKey(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            Definition.OutgoingTransform.Transform<RabbitEnvelope, RabbitEnvelope>(envelope);
            Proxy.Send(envelope);
        }

        public RabbitChannel(IChannelProxy proxy, RabbitChannelDefinition definition)
        {
            Definition = definition;
            Proxy = proxy;
        }
    }

    public class RabbitChannel<TMessage> :
        IChannel<TMessage>,
        IHaveChannelProxy
    {
        public string Name { get; set; }
        public IChannelProxy Proxy { get; set; }
        public RabbitChannelDefinition<TMessage> Definition { get; set; }

        public void ExpectReply<TReply>(TMessage message, Action<IEnvelope> modifyEnvelope, IDispatcher dispatcher, Action<TReply> onReply)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId,
                ReplyToKey = ConfigureResponseChannel<TReply>()
            };

            modifyEnvelope(envelope);
            dispatcher.ExpectResponse(envelope.MessageId.ToString(), onReply);
            Definition.OutgoingTransform.Transform<RabbitEnvelope, RabbitEnvelope>(envelope);
            Proxy.Send(envelope);
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

        public void Send(TMessage message, Action<IEnvelope> modifyEnvelope)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            modifyEnvelope(envelope);
            Definition.OutgoingTransform.Transform<RabbitEnvelope, RabbitEnvelope>(envelope);
            Proxy.Send(envelope);
        }

        public void Send(TMessage message)
        {
            var envelope = new RabbitEnvelope<TMessage>(message)
            {
                CorrelationId = Definition.CorrelationMethod(message),
                RoutingKey = Definition.RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            Definition.OutgoingTransform.Transform<RabbitEnvelope, RabbitEnvelope>(envelope);
            Proxy.Send(envelope);
        }

        public RabbitChannel(IChannelProxy proxy, RabbitChannelDefinition<TMessage> definition)
        {
            Definition = definition;
            Proxy = proxy;
        }
    }
}