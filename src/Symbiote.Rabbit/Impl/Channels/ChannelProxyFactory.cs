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

using Symbiote.Rabbit.Impl.Endpoint;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class ChannelProxyFactory : IChannelProxyFactory
    {
        private IConnectionManager _connectionManager;
        private IEndpointManager _endpointManager;
        
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

        protected IChannelProxy CreateProxy(RabbitEndpoint endpoint)
        {
            return new ChannelProxy(_connectionManager.GetConnection(endpoint.Broker).CreateModel(), _connectionManager.Protocol, endpoint);
        }

        public ChannelProxyFactory(
            IConnectionManager connectionManager, 
            IEndpointManager endpointManager)
        {
            _connectionManager = connectionManager;
            _endpointManager = endpointManager;
        }
    }
}