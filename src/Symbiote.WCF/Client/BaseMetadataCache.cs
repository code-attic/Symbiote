/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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