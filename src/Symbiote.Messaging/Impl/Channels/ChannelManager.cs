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
        protected ConcurrentDictionary<Type, List<string>> MessageChannels { get; set; }
        protected ConcurrentDictionary<Tuple<string, Type>, IChannel> Channels { get; set; }
        protected ConcurrentDictionary<Tuple<string, Type>, IChannelDefinition> Definitions { get; set; }
        protected ConcurrentDictionary<Type, IChannelFactory> ChannelFactories { get; set; }

        public void AddDefinition(IChannelDefinition definition)
        {
            Definitions.AddOrUpdate(Tuple.Create(definition.Name, definition.MessageType), definition, (x, y) => definition);
            ChannelFactories.GetOrAdd(definition.ChannelType, Assimilate.GetInstanceOf(definition.FactoryType) as IChannelFactory);
            AddChannelForMessageType(definition.MessageType, definition.Name);
        }

        public IChannelDefinition GetDefinitionFor<TMessage>(string name)
            where TMessage : class
        {
            IChannelDefinition definition = null;
            var messageType = typeof(TMessage);
            if(!Definitions.TryGetValue(Tuple.Create(name, messageType), out definition))
            {
                throw new MessagingException(
                        "There was no definition provided for a channel named {0} of message type {1}. Please check that you have defined a channel before attempting to use it."
                            .AsFormat(name, messageType));
            }
            return definition;
        }

        public IChannel<TMessage> GetChannelFor<TMessage>()
            where TMessage : class
        {
            var channelName = MessageChannels[typeof (TMessage)].First();
            return GetChannelFor<TMessage>(channelName);
        }

        public IChannel<TMessage> GetChannelFor<TMessage>(string channelName)
            where TMessage : class
        {
            IChannel channel = null;
            var messageType = typeof(TMessage);
            var key = Tuple.Create(channelName, messageType);
            if (!Channels.TryGetValue(key, out channel))
            {
                var definition = GetDefinitionFor<TMessage>(channelName);
                var factory = GetChannelFactory(definition);
                channel = factory.GetChannel(definition);
                Channels.TryAdd(key, channel);
            }
            return channel as IChannel<TMessage>;
        }

        public IChannelFactory GetChannelFactory(IChannelDefinition definition)
        {
            IChannelFactory factory = null;
            if (!ChannelFactories.TryGetValue(definition.ChannelType, out factory))
            {
                var factoryType = typeof(IChannelFactory<>).MakeGenericType(definition.ChannelType);
                factory = Assimilate.GetInstanceOf(factoryType) as IChannelFactory;
                ChannelFactories.TryAdd(factoryType, factory);
            }
            return factory;
        }

        public void AddChannelForMessageType(Type messageType, string channelName)
        {
            List<string> channels = null;
            if (!MessageChannels.TryGetValue(messageType, out channels))
            {
                channels = new List<string>();
                MessageChannels.TryAdd(messageType, channels);
            }
            if (!channels.Contains(channelName))
                channels.Add(channelName);
        }

        public ChannelManager()
        {
            Channels = new ConcurrentDictionary<Tuple<string, Type>, IChannel>();
            ChannelFactories = new ConcurrentDictionary<Type, IChannelFactory>();
            Definitions = new ConcurrentDictionary<Tuple<string, Type>, IChannelDefinition>();
            MessageChannels = new ConcurrentDictionary<Type, List<string>>();
        }
    }
}