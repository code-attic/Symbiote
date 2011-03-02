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
using org.apache.qpid.transport;
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Qpid.Impl.Channel;
using Symbiote.Qpid.Impl.Connection;
using Symbiote.Qpid.Impl.Endpoint;

namespace Symbiote.Qpid.Impl.Endpoint
{
    public class EndpointManager : IEndpointManager
    {
        protected IConnectionManager ConnectionManager { get; set; }
        protected IEndpointIndex EndpointIndex { get; set; }
        protected IChannelManager ChannelManager { get; set; }

        public void AddEndpoint(QpidEndpoint endpoint)
        {
            ChannelManager.AddDefinition(new QpidChannelDefinition(endpoint.ExchangeName, endpoint.ExchangeName));
            EndpointIndex.AddEndpoint(endpoint);
            CreateOnBroker(endpoint);
        }

        public void ConfigureEndpoint(Action<QpidEndpointFluentConfigurator> configurate)
        {
            var configurator = new QpidEndpointFluentConfigurator();
            configurate(configurator);
            var endpoint = configurator.Endpoint;
            AddEndpoint(endpoint);
        }

        public void CreateOnBroker(QpidEndpoint endpoint)
        {
            if (!endpoint.CreatedOnBroker)
            {
                var connection = ConnectionManager.GetConnection(endpoint.Broker);
                var channel = connection.CreateSession(0);

                if (!string.IsNullOrEmpty(endpoint.ExchangeName))
                    BuildExchange(channel, endpoint);

                if (!string.IsNullOrEmpty(endpoint.QueueName))
                    BuildQueue(channel, endpoint);

                if (!string.IsNullOrEmpty(endpoint.ExchangeName) && !string.IsNullOrEmpty(endpoint.QueueName))
                    BindQueue(channel, endpoint.ExchangeName, endpoint.QueueName, endpoint.RoutingKeys.ToArray());

                endpoint.CreatedOnBroker = true;
            }
        }

        public void BindQueue(string exchangeName, string queueName, params string[] routingKeys)
        {
            var endpoint = EndpointIndex.GetEndpointByExchange(exchangeName);
            var connection = ConnectionManager.GetConnection(endpoint.Broker);
            var channel = connection.CreateSession(0);
            
            BindQueue(channel, exchangeName, queueName, routingKeys);
        }

        public void BindQueue(ISession channel, string exchangeName, string queueName, params string[] routingKeys)
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

        protected void Bind(ISession channel, string exchangeName, string queueName, string routingKey, bool noWait, IDictionary args)
        {
            channel.QueueBind(queueName, exchangeName, routingKey, noWait, args);
        }

        public void BuildExchange(ISession channel, QpidEndpoint endpointConfiguration)
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

        public void BuildQueue(ISession channel, QpidEndpoint endpoint)
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

        public QpidEndpoint GetEndpointByExchange(string exchangeName)
        {
            var endpoint = EndpointIndex.GetEndpointByExchange(exchangeName);
            if (endpoint == null)
                throw new QpidConfigurationException(
                    "There was no endpoint configured for exchange {0}. Please provide configuration using the AddEndPoint method on the IBus interface.".AsFormat(exchangeName));
            CreateOnBroker(endpoint);
            return endpoint;
        }

        public QpidEndpoint GetEndpointByQueue(string queueName)
        {
            var endpoint = EndpointIndex.GetEndpointByQueue(queueName);
            if (endpoint == null)
                throw new QpidConfigurationException(
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