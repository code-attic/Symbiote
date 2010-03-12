using System;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class ChannelProxyFactory : IChannelProxyFactory
    {
        private IEndpointManager _endpointManager;
        private IConnectionManager _connectionManager;
        
        private IModel GetModel()
        {
            try
            {
                return _connectionManager.GetConnection().CreateModel();
            }
            catch (Exception e)
            {
                "The channel proxy factory could not open a channel on the current connection. \r\n\t {0}"
                    .ToError<IBus>(e);
            }
            return null;
        }

        public IChannelProxy GetProxyForQueue(string queueName)
        {
            var endpoint = _endpointManager.GetEndpointByQueue(queueName);
            return CreateProxy(endpoint);
        }

        public IChannelProxy GetProxyForExchange(string exchangeName)
        {
            var endpoint = _endpointManager.GetEndpointByExchange(exchangeName);
            return CreateProxy(endpoint);
        }

        protected IChannelProxy CreateProxy(IEndPoint endpoint)
        {
            IChannelProxy proxy = null;
            if (!endpoint.Initialized)
            {
                proxy = ConfigureEndpoint(endpoint);
                endpoint.Initialized = true;
            }
            else
            {
                proxy = new ChannelProxy(GetModel(), _connectionManager.Protocol, endpoint.EndpointConfiguration);
            }
            
            return proxy;
        }

        protected IChannelProxy ConfigureEndpoint(IEndPoint endpoint)
        {
            var channel = GetModel();
            var configuration = endpoint.EndpointConfiguration;
            if (configuration.ExchangeName != null)
                BuildExchange(channel, configuration);

            if (configuration.QueueName != null)
                BuildQueue(channel, configuration);

            if (configuration.ExchangeName != null && configuration.QueueName != null)
                ForceBindQueue(channel, configuration);

            return new ChannelProxy(channel, _connectionManager.Protocol, configuration);
        }

        protected void BindQueue(IModel channel, IAmqpEndpointConfiguration endpointConfiguration)
        {
            Action<IModel, IAmqpEndpointConfiguration, string> setup = 
                (x, y, z) => x.QueueBind(
                            y.QueueName,
                            y.ExchangeName,
                            z,
                            y.NoWait,
                            y.Arguments);

            if(endpointConfiguration.RoutingKeys.Count == 0)
            {
                setup(channel, endpointConfiguration, "");
            }
            else
            {
                endpointConfiguration.RoutingKeys.ForEach(x => setup(channel, endpointConfiguration, x));
            }
        }

        protected void BuildExchange(IModel channel, IAmqpEndpointConfiguration endpointConfiguration)
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

        protected void BuildQueue(IModel channel, IAmqpEndpointConfiguration endpointConfiguration)
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

        protected void ForceBindQueue(IModel channel, IAmqpEndpointConfiguration endpointConfiguration)
        {
            try
            {
                BindQueue(channel, endpointConfiguration);
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException e)
            {
                channel = GetModel();
                channel.ExchangeDelete(endpointConfiguration.ExchangeName, false, true);
                channel = GetModel();
                BuildExchange(channel, endpointConfiguration);
                BuildQueue(channel, endpointConfiguration);
                BindQueue(channel, endpointConfiguration);
            }
        }

        public ChannelProxyFactory(IEndpointManager endpointManager, IConnectionManager connectionManager)
        {
            _endpointManager = endpointManager;
            _connectionManager = connectionManager;
        }
    }
}