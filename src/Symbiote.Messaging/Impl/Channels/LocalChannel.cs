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
using Symbiote.Core.Impl.Futures;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Envelope;

namespace Symbiote.Messaging.Impl.Channels
{
    public class LocalChannel
        : IChannel
    {
        public string Name { get { return Definition.Name; } }
        public Func<object, string> Empty = x => "";

        public LocalChannelDefinition Definition { get; set; }

        protected IDispatcher MessageDispatcher { get; set; }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message )
        {
            return ExpectReply<TReply, TMessage>( message, x => { } );
        }

        public Future<TReply> ExpectReply<TReply, TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            Func<object, string> correlate;
            Func<object, string> route;
            Definition.CorrelationMethods.TryGetValue(typeof(TMessage), out correlate);
            Definition.RoutingMethods.TryGetValue(typeof(TMessage), out route);

            correlate = correlate ?? Empty;
            route = route ?? Empty;

            var envelope = new Envelope<TMessage>(message)
            {
                CorrelationId = correlate(message),
                RoutingKey = route(message),
            };

            modifyEnvelope(envelope);

            var future = Future.Of<TReply>( () => MessageDispatcher.Send(envelope) );
            MessageDispatcher.ExpectResponse<TReply>(envelope.MessageId.ToString(), future );
            return future;
        }

        public void Send<TMessage>( TMessage message )
        {
            Send( message, x => { } );
        }

        public void Send<TMessage>( TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            Func<object, string> correlate;
            Func<object, string> route;
            Definition.CorrelationMethods.TryGetValue(typeof(TMessage), out correlate);
            Definition.RoutingMethods.TryGetValue(typeof(TMessage), out route);

            correlate = correlate ?? Empty;
            route = route ?? Empty;

            var envelope = new Envelope<TMessage>(message)
            {
                CorrelationId = correlate(message),
                RoutingKey = route(message),
            };

            modifyEnvelope(envelope);

            MessageDispatcher.Send(envelope);
        }

        public LocalChannel( IDispatcher messageDispatcher, LocalChannelDefinition definition )
        {
            Definition = definition;
            MessageDispatcher = messageDispatcher;
        }
    }
}