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
using Symbiote.Core.Utility;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl
{
    public class Bus
        : IBus
    {
        protected IChannelIndex ChannelIndex { get; set; }
        protected IChannelManager Channels { get; set; }
        protected ISubscriptionManager Subscriptions { get; set; }

        public bool HasChannelFor<T>()
        {
            return ChannelIndex.HasChannelFor<T>();
        }

        public bool HasChannelFor<T>(string channelName)
        {
            return ChannelIndex.HasChannelFor<T>(channelName);
        }

        public void Publish<TMessage>(TMessage message)
        {
            var channelsFor = Channels
                .GetChannelsFor<TMessage>();

            channelsFor
                .ForEach(x => x.Send(message));
        }

        public void Publish<TMessage>( string channelName, TMessage message )
        {
            var channelFor = Channels
                .GetChannelFor<TMessage>( channelName );

            channelFor
                .Send( message );
        }

        public void Publish<TMessage>( string channelName, TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var channelFor = Channels
                .GetChannelFor<TMessage>(channelName);

            channelFor
                .Send(message, modifyEnvelope);
        }

        public Future<TResponse> Request<TMessage, TResponse>(string channelName, TMessage message )
        {
            var channelFor = Channels
                .GetChannelFor<TMessage>(channelName);

            return channelFor.ExpectReply<TResponse, TMessage>( message, x => { });
        }

        public Future<TResponse> Request<TMessage, TResponse>( string channelName, TMessage message, Action<IEnvelope> modifyEnvelope )
        {
            var channelFor = Channels
                .GetChannelFor<TMessage>(channelName);

            return channelFor.ExpectReply<TResponse, TMessage>(message, modifyEnvelope);
        }

        public void StartSubscription(string subscription)
        {
            Subscriptions.StartSubscription(subscription);
        }

        public void StopSubscription(string subscription)
        {
            Subscriptions.StopSubscription(subscription);
        }

        public Bus(
            IChannelManager channels,
            IChannelIndex channelIndex,
            ISubscriptionManager subscriptions)
        {
            Channels = channels;
            ChannelIndex = channelIndex;
            Subscriptions = subscriptions;
        }
    }
}