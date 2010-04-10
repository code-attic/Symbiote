using System;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class ChannelProxyFactory : IChannelProxyFactory
    {
        private IEndpointIndex _endpointIndex;
        private IConnectionManager _connectionManager;
        private IEndpointManager _endpointManager;
        
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
            var endpoint = _endpointIndex.GetEndpointByQueue(queueName);
            _endpointManager.ConfigureEndpoint(endpoint);
            return CreateProxy(endpoint);
        }

        public IChannelProxy GetProxyForExchange(string exchangeName)
        {
            var endpoint = _endpointIndex.GetEndpointByExchange(exchangeName);
            _endpointManager.ConfigureEndpoint(endpoint);
            return CreateProxy(endpoint);
        }

        protected IChannelProxy CreateProxy(IEndPoint endpoint)
        {
            return new ChannelProxy(GetModel(), _connectionManager.Protocol, endpoint.EndpointConfiguration);
        }

        public ChannelProxyFactory(IEndpointIndex endpointIndex, IConnectionManager connectionManager, IEndpointManager endpointManager)
        {
            _endpointIndex = endpointIndex;
            _connectionManager = connectionManager;
            _endpointManager = endpointManager;
        }
    }
}