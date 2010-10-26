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
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Rabbit.Impl.Adapter;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class EndpointManager : IEndpointManager
    {
        protected IConnectionManager ConnectionManager { get; set; }
        protected IEndpointIndex EndpointIndex { get; set; }
        protected IChannelManager ChannelManager { get; set; }

        public void AddEndpoint<TMessage>(RabbitEndpoint endpoint)
        {
            ChannelManager.AddDefinition(new RabbitChannelDefinition<TMessage>(endpoint.ExchangeName));
            EndpointIndex.AddEndpoint<TMessage>(endpoint);
            CreateOnBroker(endpoint);
        }

        public void ConfigureEndpoint<TMessage>(Action<RabbitEndpointFluentConfigurator> configurate)
        {
            var configurator = new RabbitEndpointFluentConfigurator();
            configurate(configurator);
            var endpoint = configurator.Endpoint;
            AddEndpoint<TMessage>(endpoint);

            if (!string.IsNullOrEmpty(endpoint.QueueName))
            {
                var subscriptions = Assimilate.GetInstanceOf<ISubscriptionManager>();
                var queueSubscription = Assimilate.GetInstanceOf<QueueSubscription<TMessage>>();
                queueSubscription.Name = endpoint.QueueName;
                if(configurator.Subscribe) 
                    subscriptions.AddAndStartSubscription(queueSubscription);
                else
                    subscriptions.AddSubscription(queueSubscription);
            }
        }

        public void CreateOnBroker(RabbitEndpoint endpoint)
        {
            if (!endpoint.CreatedOnBroker)
            {
                var connection = ConnectionManager.GetConnection(endpoint.Broker);
                using (var channel = connection.CreateModel())
                {

                    if (!string.IsNullOrEmpty(endpoint.ExchangeName))
                        BuildExchange(channel, endpoint);

                    if (!string.IsNullOrEmpty(endpoint.QueueName))
                        BuildQueue(channel, endpoint);

                    if (!string.IsNullOrEmpty(endpoint.ExchangeName) && !string.IsNullOrEmpty(endpoint.QueueName))
                        BindQueue(channel, endpoint.ExchangeName, endpoint.QueueName, endpoint.RoutingKeys.ToArray());

                    endpoint.CreatedOnBroker = true;
                }
            }
        }

        public void BindQueue(string exchangeName, string queueName, params string[] routingKeys)
        {
            var endpoint = EndpointIndex.GetEndpointByExchange(exchangeName);
            var connection = ConnectionManager.GetConnection(endpoint.Broker);
            using (var channel = connection.CreateModel())
            {
                BindQueue(channel, exchangeName, queueName, routingKeys);
            }
        }

        public void BindQueue(IModel channel, string exchangeName, string queueName, params string[] routingKeys)
        {
            if(routingKeys.Length == 0)
                routingKeys = new [] {""};
            try
            {
                routingKeys
                    .ForEach(x => Bind(channel, exchangeName, queueName, x, false, null));
            }
            catch (Exception x)
            {
                
                throw;
            }
        }

        protected void Bind(IModel channel, string exchangeName, string queueName, string routingKey, bool noWait, IDictionary args)
        {
            channel.QueueBind(queueName, exchangeName, routingKey, noWait, args);
        }

        public void BuildExchange(IModel channel, RabbitEndpoint endpointConfiguration)
        {
            channel.ExchangeDeclare(
                endpointConfiguration.ExchangeName,
                endpointConfiguration.ExchangeTypeName,
                endpointConfiguration.Passive,
                endpointConfiguration.Durable,
                endpointConfiguration.AutoDelete,
                endpointConfiguration.Internal,
                endpointConfiguration.NoWait,
                endpointConfiguration.Arguments);
        }

        public void BuildQueue(IModel channel, RabbitEndpoint endpoint)
        {
            channel.QueueDeclare(
                endpoint.QueueName,
                endpoint.Passive,
                endpoint.Durable,
                endpoint.Exclusive,
                endpoint.AutoDelete,
                endpoint.NoWait,
                endpoint.Arguments);
        }

        public RabbitEndpoint GetEndpointByExchange(string exchangeName)
        {
            var endpoint = EndpointIndex.GetEndpointByExchange(exchangeName);
            if (endpoint == null)
                throw new RabbitConfigurationException(
                    "There was no endpoint configured for exchange {0}. Please provide configuration using the AddEndPoint method on the IBus interface.".AsFormat(exchangeName));
            CreateOnBroker(endpoint);
            return endpoint;
        }

        public RabbitEndpoint GetEndpointByQueue(string queueName)
        {
            var endpoint = EndpointIndex.GetEndpointByQueue(queueName);
            if (endpoint == null)
                throw new RabbitConfigurationException(
                    "There was no endpoint configured for exchange {0}. Please provide configuration using the AddEndPoint method on the IBus interface.".AsFormat(queueName));
            CreateOnBroker(endpoint);
            return endpoint;
        }

        public EndpointManager(IChannelManager channelManager, IConnectionManager connectionManager, IEndpointIndex endpointIndex)
        {
            ConnectionManager = connectionManager;
            EndpointIndex = endpointIndex;
            ChannelManager = channelManager;
        }
    }
}