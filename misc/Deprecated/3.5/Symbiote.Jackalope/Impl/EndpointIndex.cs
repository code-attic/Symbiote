using System.Collections.Generic;
using System.Threading;

namespace Symbiote.Jackalope.Impl
{
    public class EndpointIndex : IEndpointIndex
    {
        private ReaderWriterLockSlim _queueLock = new ReaderWriterLockSlim();
        private ReaderWriterLockSlim _exchangeLock = new ReaderWriterLockSlim();
        private Dictionary<string, IEndPoint> _endpointsByQueue = new Dictionary<string, IEndPoint>();
        private Dictionary<string, IEndPoint> _endpointsByExchange = new Dictionary<string, IEndPoint>();

        public IEndPoint GetEndpointByExchange(string exchangeName)
        {
            IEndPoint endpoint = null;
            _exchangeLock.EnterReadLock();
            _endpointsByExchange.TryGetValue(exchangeName, out endpoint);
            _exchangeLock.ExitReadLock();
            return endpoint;
        }

        public IEndPoint GetEndpointByQueue(string queueName)
        {
            IEndPoint endpoint = null;
            _queueLock.EnterReadLock();
            _endpointsByQueue.TryGetValue(queueName, out endpoint);
            _queueLock.ExitReadLock();
            return endpoint;
        }

        public void AddEndpoint(IEndPoint endpoint)
        {
            if(!string.IsNullOrEmpty(endpoint.EndpointConfiguration.ExchangeName))
            {
                _exchangeLock.EnterWriteLock();
                _endpointsByExchange[endpoint.EndpointConfiguration.ExchangeName] = endpoint;
                _exchangeLock.ExitWriteLock();
            }
            if(!string.IsNullOrEmpty(endpoint.EndpointConfiguration.QueueName))
            {
                _queueLock.EnterWriteLock();
                _endpointsByQueue[endpoint.EndpointConfiguration.QueueName] = endpoint;
                _queueLock.ExitWriteLock();
            }
        }
    }


}