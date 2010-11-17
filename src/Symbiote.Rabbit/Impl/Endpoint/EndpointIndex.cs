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
using Symbiote.Core.Extensions;

namespace Symbiote.Rabbit.Impl.Endpoint
{
    public class EndpointIndex : IEndpointIndex
    {
        private ConcurrentDictionary<string, RabbitEndpoint> _endpointsByQueue = new ConcurrentDictionary<string, RabbitEndpoint>();
        
        public RabbitEndpoint GetEndpointByQueue(string queueName)
        {
            RabbitEndpoint endpoint = null;
            _endpointsByQueue.TryGetValue(queueName, out endpoint);
            if (endpoint == null)
                throw new ConfigurationException(
                    "There was no endpoint configured for queue {0}. Please provide configuration using the AddEndPoint method on the IBus interface.".AsFormat(queueName));
            return endpoint;
        }

        public void AddEndpoint(RabbitEndpoint endpoint)
        {
            _endpointsByQueue[endpoint.QueueName] = endpoint;
        }
    }
}