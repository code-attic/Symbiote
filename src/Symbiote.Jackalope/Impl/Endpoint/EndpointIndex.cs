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