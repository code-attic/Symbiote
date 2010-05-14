using System;
using System.Collections.Generic;
using System.ServiceModel.Description;

namespace Symbiote.WCFDynamicClient.Impl
{
    public class InMemoryMetadataCache : BaseMetadataCache
    {
        private Dictionary<Tuple<string, string>, ServiceEndpoint> _endpointCache = new Dictionary<Tuple<string, string>, ServiceEndpoint>();

        protected override ServiceEndpoint GetEndPointFromStore<TContract>(string metadataExchange)
        {
            var key = Tuple.Create(typeof(TContract).FullName, metadataExchange);
            ServiceEndpoint endpoint = null;
            _endpointCache.TryGetValue(key, out endpoint);
            return endpoint;
        }

        protected override void SetEndPointFromStore<TContract>(string metadataExchange, ServiceEndpoint endpoint)
        {
            var key = Tuple.Create(typeof(TContract).FullName, metadataExchange);
            _endpointCache.Add(key, endpoint);
        }
    }
}