using System;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Impl.Endpoint;
using Symbiote.Jackalope.Impl.Server;

namespace Symbiote.Jackalope.Impl.Channel
{
    public class ChannelProxyFactory : IChannelProxyFactory
    {
        private IConnectionManager _connectionManager;
        private IEndpointManager _endpointManager;
        
        public IChannelProxy GetProxyForQueue(string queueName)
        {
            var endpoint = _endpointManager.GetEndpointByQueue(queueName);
            return CreateProxy(endpoint);
        }

        public IChannelProxy GetProxyForQueue<T>(string queueName, T id)
        {
            var endpoint = _endpointManager.GetEndpointByQueue(queueName);
            return CreateProxy(endpoint, id);
        }

        public IChannelProxy GetProxyForExchange(string exchangeName)
        {
            var endpoint = _endpointManager.GetEndpointByExchange(exchangeName);
            return CreateProxy(endpoint);
        }

        public IChannelProxy GetProxyForExchange<T>(string exchangeName, T id)
        {
            var endpoint = _endpointManager.GetEndpointByExchange(exchangeName);
            return CreateProxy(endpoint, id);
        }

        protected IChannelProxy CreateProxy(IEndPoint endpoint)
        {
            return new ChannelProxy(_connectionManager.GetConnection().CreateModel(), _connectionManager.Protocol, endpoint.EndpointConfiguration);
        }

        protected IChannelProxy CreateProxy<T>(IEndPoint endpoint, T id)
        {
            return new ChannelProxy(_connectionManager.GetConnection(id).CreateModel(), _connectionManager.Protocol, endpoint.EndpointConfiguration);
        }

        public ChannelProxyFactory(
            IConnectionManager connectionManager, 
            IEndpointManager endpointManager)
        {
            _connectionManager = connectionManager;
            _endpointManager = endpointManager;
        }
    }
}