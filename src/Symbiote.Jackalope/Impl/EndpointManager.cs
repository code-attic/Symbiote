using System.Collections.Generic;

namespace Symbiote.Jackalope.Impl
{
    public class EndpointManager : IEndpointManager
    {
        private Dictionary<string, IEndPoint> _endpointsByQueue = new Dictionary<string, IEndPoint>();
        private Dictionary<string, IEndPoint> _endpointsByExchange = new Dictionary<string, IEndPoint>();

        public IEndPoint GetEndpointByExchange(string exchangeName)
        {
            IEndPoint endpoint = null;
            _endpointsByExchange.TryGetValue(exchangeName, out endpoint);
            return endpoint;
        }

        public IEndPoint GetEndpointByQueue(string queueName)
        {
            IEndPoint endpoint = null;
            _endpointsByQueue.TryGetValue(queueName, out endpoint);
            return endpoint;
        }

        public void AddEndpoint(IEndPoint endpoint)
        {
            if(!string.IsNullOrEmpty(endpoint.EndpointConfiguration.ExchangeName))
            {
                _endpointsByExchange[endpoint.EndpointConfiguration.ExchangeName] = endpoint;
            }
            if(!string.IsNullOrEmpty(endpoint.EndpointConfiguration.QueueName))
            {
                _endpointsByQueue[endpoint.EndpointConfiguration.QueueName] = endpoint;
            }
        }
    }
}