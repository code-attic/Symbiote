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
using System.Collections.Generic;
using System.ServiceModel.Description;

namespace Symbiote.Wcf.Client
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