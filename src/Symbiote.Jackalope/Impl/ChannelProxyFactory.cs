using System;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Impl
{
    public class ChannelProxyFactory : IChannelProxyFactory
    {
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
            return new ChannelProxy(GetModel(), _connectionManager.Protocol, endpoint.EndpointConfiguration);
        }

        public ChannelProxyFactory(IConnectionManager connectionManager, IEndpointManager endpointManager)
        {
            _connectionManager = connectionManager;
            _endpointManager = endpointManager;
        }
    }
}