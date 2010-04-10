using System;
using System.Collections;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class EndpointManager : IEndpointManager
    {
        protected IConnectionManager _connectionManager;

        public void ConfigureEndpoint(IEndPoint endpoint)
        {
            var configuration = endpoint.EndpointConfiguration;
            using(var channel = _connectionManager.GetConnection().CreateModel())
            {
                if (!endpoint.Initialized)
                {
                    if (!string.IsNullOrEmpty(configuration.ExchangeName))
                        BuildExchange(channel, configuration);

                    if (!string.IsNullOrEmpty(configuration.QueueName))
                        BuildQueue(channel, configuration);

                    if (!string.IsNullOrEmpty(configuration.ExchangeName) && !string.IsNullOrEmpty(configuration.QueueName))
                        BindQueue(channel, configuration.ExchangeName, configuration.QueueName, configuration.RoutingKeys.ToArray());

                    endpoint.Initialized = true;
                }
            }
        }

        public void BindQueue(string exchangeName, string queueName, params string[] routingKeys)
        {
            var channel = _connectionManager.GetConnection().CreateModel();
            BindQueue(channel, exchangeName, queueName, routingKeys);
        }

        public void BindQueue(IModel channel, string exchangeName, string queueName, params string[] routingKeys)
        {
            if(routingKeys.Length == 0)
                routingKeys = new [] {""};
            try
            {
                routingKeys
                    .ForEach(x => Bind(channel, exchangeName, queueName, x, true, null));
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        protected void Bind(IModel channel, string exchangeName, string queueName, string routingKey, bool noWait, IDictionary args)
        {
            channel.QueueBind(queueName, exchangeName, routingKey, noWait, args);
        }

        public void BuildExchange(IModel channel, IAmqpEndpointConfiguration endpointConfiguration)
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

        public void BuildQueue(IModel channel, IAmqpEndpointConfiguration endpointConfiguration)
        {
            channel.QueueDeclare(
                endpointConfiguration.QueueName,
                endpointConfiguration.Passive,
                endpointConfiguration.Durable,
                endpointConfiguration.Exclusive,
                endpointConfiguration.AutoDelete,
                endpointConfiguration.NoWait,
                endpointConfiguration.Arguments);
        }
        
        public EndpointManager(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }
    }
}