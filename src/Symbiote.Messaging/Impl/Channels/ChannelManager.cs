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
using System.Collections.Concurrent;
using Symbiote.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Messaging.Impl.Channels
{
    public class ChannelManager
        : IChannelManager
    {
        protected ConcurrentDictionary<string, Type> ChannelTypes { get; set; }
        protected ConcurrentDictionary<string, IChannel> Channels { get; set; }
        protected ConcurrentDictionary<string, IChannelDefinition> Definitions { get; set; }
        protected ConcurrentDictionary<Type, IChannelFactory> ChannelFactories { get; set; }

        public void AddDefinition(IChannelDefinition definition)
        {
            Definitions.AddOrUpdate(definition.Name, definition, (x, y) => definition);
        }

        public IChannel GetChannel(string channelName)
        {
            IChannel channel = null;
            if(!Channels.TryGetValue(channelName, out channel))
            {
                var factory = GetChannelFactory(channelName);
                IChannelDefinition definition = null;
                if(!Definitions.TryGetValue(channelName, out definition))
                {
                    throw new MessagingException(
                        "There was no definition provided for a channel named {0}. Please check that you have defined a channel before attempting to use it."
                            .AsFormat(channelName));
                }
                channel = factory.GetChannel(definition);
                Channels.TryAdd(channelName, channel);
            }
            return channel;
        }

        public IChannelFactory GetChannelFactory(string channelName)
        {
            Type channelType = null;
            if(!ChannelTypes.TryGetValue(channelName, out channelType))
            {
                channelType = typeof (LocalChannel);
                ChannelTypes.TryAdd(channelName, channelType);
            }

            IChannelFactory factory = null;
            if (!ChannelFactories.TryGetValue(channelType, out factory))
            {
                var factoryType = typeof(IChannelFactory<>).MakeGenericType(channelType);
                factory = ServiceLocator.Current.GetInstance(factoryType) as IChannelFactory;
                //factory = factory ?? ServiceLocator.Current.GetInstance(typeof (ChannelFactory<>)) as IChannelFactory;
                ChannelFactories.TryAdd(factoryType, factory);
            }
            return factory;
        }

        public ChannelManager()
        {
            ChannelTypes = new ConcurrentDictionary<string, Type>();
            Channels = new ConcurrentDictionary<string, IChannel>();
            ChannelFactories = new ConcurrentDictionary<Type, IChannelFactory>();
            Definitions = new ConcurrentDictionary<string, IChannelDefinition>();
        }
    }
}