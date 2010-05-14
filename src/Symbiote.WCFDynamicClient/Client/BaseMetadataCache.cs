using System;
using System.Linq;
using System.ServiceModel.Description;

namespace Symbiote.Wcf.Client
{
    public abstract class BaseMetadataCache : IServiceMetadataCache
    {
        protected abstract ServiceEndpoint GetEndPointFromStore<TContract>(string metadataExchange);
        protected abstract void SetEndPointFromStore<TContract>(string metadataExchange, ServiceEndpoint endpoint);

        public ServiceEndpoint GetEndPoint<TContract>(string metadataExchange)
        {
            ServiceEndpoint endpoint = GetEndPointFromStore<TContract>(metadataExchange);
            if (endpoint == null)
            {
                endpoint = GetEndPointFromExchange<TContract>(metadataExchange);
                SetEndPointFromStore<TContract>(metadataExchange, endpoint);
            }
            return endpoint;
        }

        protected ServiceEndpoint GetEndPointFromExchange<TContract>(string metadataExchange)
        {
            var serviceInfo = MetadataResolver.Resolve(typeof(TContract), new Uri(metadataExchange),
                                                       MetadataExchangeClientMode.HttpGet);

            if (serviceInfo.Count == 0)
                throw new ServiceClientException(string.Format("No service endpoints defined for {0}", metadataExchange));

            return serviceInfo.First();
        }
    }
}