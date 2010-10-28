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
using Symbiote.Messaging;
using Symbiote.Messaging.Extensions;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Rabbit.Config;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class RabbitChannel<TMessage>
        : IChannel<TMessage>
    {
        public IChannelProxy Proxy { get; set; }

        public string Name { get; set; }
        public Func<TMessage, string> RoutingMethod { get; set; }
        public Func<TMessage, string> CorrelationMethod { get; set; }

        public IEnvelope<TMessage> Send(TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope)
        {
            var envelope = new RabbitEnvelope<TMessage>()
            {
                Message = message,
                CorrelationId = CorrelationMethod( message ),
                RoutingKey = RoutingMethod( message ),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            modifyEnvelope( envelope );

            Proxy.Send(envelope);
            return envelope;
        }

        public IEnvelope<TMessage> Send(TMessage message)
        {
            var envelope = new RabbitEnvelope<TMessage>()
            {
                Message = message,
                CorrelationId = CorrelationMethod(message),
                RoutingKey = RoutingMethod(message),
                ReplyToExchange = RabbitBroker.ResponseId
            };

            Proxy.Send(envelope);
            return envelope;
        }

        public RabbitChannel(IChannelProxy proxy)
        {
            Proxy = proxy;
        }
    }
}