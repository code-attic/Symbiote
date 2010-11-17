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

using Symbiote.Messaging.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class ChannelProxyFactory : IChannelProxyFactory
    {
        private IConnectionManager ConnectionManager { get; set; }
        
        public IChannelProxy GetProxyForQueue(RabbitEndpoint endpoint)
        {
            endpoint.CreateOnBroker(ConnectionManager);
            return new ChannelProxy(ConnectionManager.GetConnection(endpoint.Broker).CreateModel(), ConnectionManager.Protocol, endpoint);
        }

        public IChannelProxy GetProxyForExchange(ChannelDefinition channelDefinition)
        {
            channelDefinition.CreateOnBroker(ConnectionManager);
            return new ChannelProxy(ConnectionManager.GetConnection(channelDefinition.Broker).CreateModel(), ConnectionManager.Protocol, channelDefinition);
        }

        public ChannelProxyFactory(
            IConnectionManager connectionManager)
        {
            ConnectionManager = connectionManager;
        }
    }
}