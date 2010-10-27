﻿/* 
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
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Envelope;

namespace Symbiote.Messaging.Impl.Channels
{
    public class LocalChannel<TMessage>
        : IChannel<TMessage>
    {
        protected IDispatcher messageDispatcher { get; set; }

        public string Name { get; set; }
        public Func<TMessage, string> RoutingMethod { get; set; }
        public Func<TMessage, string> CorrelationMethod { get; set; }

        public void Send(TMessage message)
        {
            var envelope = new Envelope<TMessage>(message)
            {
                CorrelationId = CorrelationMethod(message),
                RoutingKey = RoutingMethod(message),
            };

            messageDispatcher.Send(envelope);
        }

        public void Send(TMessage message, Action<IEnvelope<TMessage>> modifyEnvelope)
        {
            var envelope = new Envelope<TMessage>(message)
            {
                CorrelationId = CorrelationMethod(message),
                RoutingKey = RoutingMethod(message),
            };

            modifyEnvelope(envelope);

            messageDispatcher.Send(envelope);
        }

        public LocalChannel(IDispatcher messageDirector)
        {
            messageDispatcher = messageDirector;
        }
    }
}