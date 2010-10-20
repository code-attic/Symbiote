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
using System.Linq;
using System.Text;
using Symbiote.Messaging.Impl.Channels;

namespace Symbiote.Rabbit.Impl.Channels
{
    public class RabbitChannelFactory
        : IChannelFactory
    {
        public IChannelProxyFactory ProxyFactory { get; set; }

        public IChannel GetChannel(IChannelDefinition definition)
        {
            var rabbitDef = definition as IRabbitChannelDefinition;
            var proxy = ProxyFactory.GetProxyForExchange(rabbitDef.Exchange);
            return Activator.CreateInstance(rabbitDef.ChannelType, proxy) as IChannel;
        }

        public RabbitChannelFactory(IChannelProxyFactory proxyFactory)
        {
            ProxyFactory = proxyFactory;
        }
    }
}
