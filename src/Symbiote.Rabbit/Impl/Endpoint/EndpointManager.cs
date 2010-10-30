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
using System.Collections;
using RabbitMQ.Client;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Rabbit.Impl.Adapter;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Server;
using Symbiote.Rabbit.Impl.Subscription;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class EndpointManager : IEndpointManager
    {
        protected IEndpointIndex EndpointIndex { get; set; }
        protected IChannelManager ChannelManager { get; set; }
        protected QueueSubscriptionFactory SubscriptionFactory { get; set; }

        public void AddEndpoint<TMessage>(RabbitEndpoint endpoint)
        {
            EndpointIndex.AddEndpoint<TMessage>(endpoint);
        }

        public void ConfigureEndpoint<TMessage>(Action<RabbitEndpointFluentConfigurator<TMessage>> configurate)
        {
            var configurator = new RabbitEndpointFluentConfigurator<TMessage>();
            configurate(configurator);
            var endpoint = configurator.Endpoint;
            AddEndpoint<TMessage>(endpoint);
            ChannelManager.AddDefinition( configurator.ChannelDefinition );

            if (!string.IsNullOrEmpty(endpoint.QueueName))
            {
                SubscriptionFactory.CreateSubscription<TMessage>( configurator.ChannelDefinition, configurator.Subscribe );
            }
        }

        public EndpointManager(
            IChannelManager channelManager, 
            IEndpointIndex endpointIndex,
            QueueSubscriptionFactory subscriptionFactory)
        {
            EndpointIndex = endpointIndex;
            ChannelManager = channelManager;
            SubscriptionFactory = subscriptionFactory;
        }
    }
}