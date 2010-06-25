using System.Collections.Concurrent;

namespace Symbiote.Jackalope.Impl.Endpoint
{
    public class EndpointIndex : IEndpointIndex
    {
        private ConcurrentDictionary<string, IEndPoint> _endpointsByQueue = new ConcurrentDictionary<string, IEndPoint>();
        private ConcurrentDictionary<string, IEndPoint> _endpointsByExchange = new ConcurrentDictionary<string, IEndPoint>();

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