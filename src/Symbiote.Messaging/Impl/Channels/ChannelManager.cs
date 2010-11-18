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
using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using System.Linq;

namespace Symbiote.Messaging.Impl.Channels
{
    public class ChannelManager
        : IChannelManager
    {
        public ConcurrentDictionary<int, IChannel> Channels { get; set; }
        public ConcurrentDictionary<Type, IChannelFactory> ChannelFactories { get; set; }
        public IChannelIndex Index { get; set; }

        public IChannel CreateChannelInstance( int key, IChannelDefinition definition )
        {
            var factory = GetChannelFactory(definition);
            IChannel channel = factory.CreateChannel(definition);
            Channels.TryAdd(key, channel);
            return channel;
        }

        public IChannel GetChannelFor<TMessage>()
        {
            var messageType = typeof(TMessage);
            var adapter = GetChannelsFor<TMessage>().FirstOrDefault();

            if(adapter == null)
                throw new MissingChannelDefinitionException(
                    "There was no definition provided for a channel named {0} of message type {1}. Please check that you have defined a channel before attempting to use it."
                        .AsFormat("<nothing>", messageType));

            return adapter;
        }

        public IChannel GetChannelFor<TMessage>(string channelName)
        {
            IChannel channel;
            int key = Index.GetKeyFor<TMessage>( channelName );
            if (!Channels.TryGetValue(key, out channel))
            {
                var definition = Index.GetDefinitionFor<TMessage>(channelName);
                channel = CreateChannelInstance( key, definition );
            }
            return channel;
        }

        public IEnumerable<IChannel> GetChannelsFor<TMessage>()
        {
            var adapters = new List<IChannel>();
            var definitions = Index.GetDefinitionsFor<TMessage>();

            foreach (var definition in definitions)
            {
                IChannel channel;
                var key = Index.GetKeyFor<TMessage>( definition.Name );
                if(!Channels.TryGetValue( key, out channel ))
                {
                    channel = CreateChannelInstance(key, definition);
                }
                adapters.Add(channel);
            }
            return adapters;
        }

        public IChannelFactory GetChannelFactory(IChannelDefinition definition)
        {
            IChannelFactory factory;
            if(!ChannelFactories.TryGetValue(definition.ChannelType, out factory ))
            {
                factory = Assimilate.GetInstanceOf( definition.FactoryType ) as IChannelFactory;
                ChannelFactories.TryAdd(definition.FactoryType,
                                         factory);
            }
            return factory;
        }

        public ChannelManager(IChannelIndex channelIndex)
        {
            Index = channelIndex;
            Channels = new ConcurrentDictionary<int, IChannel>();
            ChannelFactories = new ConcurrentDictionary<Type, IChannelFactory>();
        }
    }
}