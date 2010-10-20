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
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl
{
    public class Bus
        : IBus
    {
        protected IChannelManager Channels { get; set; }
        protected ISubscriptionManager Subscriptions { get; set; }

        public void AddSubscription(ISubscription subscription)
        {
            Subscriptions.AddSubscription(subscription);
        }

        public bool HasChannelFor<T>()
        {
            return Channels.HasChannelFor<T>();
        }

        public bool HasChannelFor<T>(string channelName)
        {
            return Channels.HasChannelFor<T>(channelName);
        }

        public void Send<TMessage>(string channelName, TMessage message)
            where TMessage : class
        {
            var channel = Channels.GetChannelFor<TMessage>(channelName);
            channel.Send(message);
        }

        public void Send<TMessage>(TMessage message)
            where TMessage : class
        {
            Channels
                .GetChannelsFor<TMessage>()
                .ForEach(x => x.Send(message));
        }

        public TReply Query<TMessage, TReply>(string channelName, TMessage message)
            where TMessage : class
        {
            var channel = Channels.GetChannelFor<TMessage>(channelName);
            channel.Send(message);
        }

        public void Query<TMessage>(TMessage message)
            where TMessage : class
        {
            Channels
                .GetChannelsFor<TMessage>()
                .ForEach(x => x.Send(message));
        }

        public void StartSubscription(string subscription)
        {
            Subscriptions.StartSubscription(subscription);
        }

        public void StopSubscription(string subscription)
        {
            Subscriptions.StopSubscription(subscription);
        }

        public Bus(IChannelManager channels, ISubscriptionManager subscriptions)
        {
            Channels = channels;
            Subscriptions = subscriptions;
        }
    }
}