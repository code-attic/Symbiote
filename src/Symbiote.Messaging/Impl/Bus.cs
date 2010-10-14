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

        public void Send<TMessage>(string channelName, TMessage message)
            where TMessage : class
        {
            var channel = Channels.GetChannel(channelName);
            channel.Send(message);
        }

        public Bus(IChannelManager channels, ISubscriptionManager subscriptions)
        {
            Channels = channels;
            Subscriptions = subscriptions;
        }
    }
}