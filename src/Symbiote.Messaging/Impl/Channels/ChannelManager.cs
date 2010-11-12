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
        public ConcurrentDictionary<Type, List<string>> MessageChannels { get; set; }
        public ConcurrentDictionary<int, IChannel> Channels { get; set; }
        public ConcurrentDictionary<int, IChannelDefinition> Definitions { get; set; }
        public ConcurrentDictionary<Type, IChannelFactory> ChannelFactories { get; set; }

        public void AddDefinition(IChannelDefinition definition)
        {
            ValidateChannelDefinition( definition );

            Definitions.AddOrUpdate(
                GetChannelKey(definition.MessageType, definition.Name),
                definition,
                (x, y) => definition);

            ChannelFactories.GetOrAdd(definition.ChannelType,
                                       Assimilate.GetInstanceOf(definition.FactoryType) as IChannelFactory);
            AddChannelForMessageType(definition.MessageType,
                                      definition.Name);
        }

        public IChannelDefinition GetDefinitionFor<TMessage>(string name)
        {
            IChannelDefinition definition = null;
            var messageType = typeof(TMessage);
            if (!Definitions.TryGetValue(
                                GetChannelKey(
                                    messageType,
                                    name),
                                out definition) &&
                !Definitions.TryGetValue(
                                GetChannelKey( 
                                    typeof(object),
                                    name),
                                out definition)
                )
            {
                throw new MissingChannelDefinitionException(
                    "There was no definition provided for a channel named {0} of message type {1}. Please check that you have defined a channel before attempting to use it."
                        .AsFormat(name, messageType));
            }
            return definition;
        }

        protected int GetChannelKey(Type messageType, string name)
        {
            return (name.GetHashCode() * 397) ^ messageType.GetHashCode();
        }

        public IChannelAdapter GetChannelFor<TMessage>()
        {
            var messageType = typeof(TMessage);
            var channelName = MessageChannels[messageType].FirstOrDefault() ??
                              MessageChannels[typeof(object)].FirstOrDefault();

            if(string.IsNullOrEmpty(channelName))
                throw new MissingChannelDefinitionException(
                    "There was no definition provided for a channel named {0} of message type {1}. Please check that you have defined a channel before attempting to use it."
                        .AsFormat("<nothing>", messageType));


            return GetChannelFor<TMessage>(channelName);
        }

        public IEnumerable<IChannelAdapter> GetChannelsFor<TMessage>()
        {
            List<string> channelNameList;
            if (!MessageChannels.TryGetValue(typeof(TMessage), out channelNameList) &&
                !MessageChannels.TryGetValue(typeof(object), out channelNameList))
            {
                channelNameList = new List<string>();    
            }
            
            return channelNameList
                .Select(GetChannelFor<TMessage>);
        }

        public IChannelAdapter GetChannelFor<TMessage>(string channelName)
        {
            IChannel channel = null;
            var key = GetChannelKey(typeof(TMessage),
                                     channelName);

            var openKey = GetChannelKey( typeof(Object), channelName );
            if (!Channels.TryGetValue(key, out channel) &&
                !Channels.TryGetValue(openKey, out channel))
            {
                var definition = GetDefinitionFor<TMessage>(channelName);
                var factory = GetChannelFactory(definition);
                channel = factory.CreateChannel(definition);
                Channels.TryAdd(key, channel);
            }
            var adapter = channel.GetType().GetInterface( "IChannel`1" ) == null
                              ? new ChannelAdapter( channel as IOpenChannel ) as IChannelAdapter
                              : new ChannelAdapter<TMessage>( channel as IChannel<TMessage> );
            return adapter;
        }

        public IChannelFactory GetChannelFactory(IChannelDefinition definition)
        {
            IChannelFactory factory = null;
            if(definition as IOpenChannelDefinition != null 
                && !ChannelFactories.TryGetValue(definition.ChannelType, out factory ))
            {
                factory = Assimilate.GetInstanceOf( definition.FactoryType ) as IChannelFactory;
                ChannelFactories.TryAdd(definition.FactoryType,
                                         factory);
            }
            else if (!ChannelFactories.TryGetValue(definition.ChannelType,
                                                out factory))
            {
                var factoryType = typeof(IChannelFactory<>).MakeGenericType(definition.ChannelType);
                factory = Assimilate.GetInstanceOf(factoryType) as IChannelFactory;
                ChannelFactories.TryAdd(factoryType,
                                         factory);
            }
            return factory;
        }

        public bool HasChannelFor<TMessage>()
        {
            return MessageChannels.ContainsKey(typeof(TMessage)) || MessageChannels.ContainsKey( typeof(object) );
        }

        public bool HasChannelFor<TMessage>(string channelName)
        {
            return (MessageChannels.ContainsKey(typeof(TMessage)) &&
                   MessageChannels[typeof(TMessage)].Contains(channelName)) ||
                   (MessageChannels.ContainsKey( typeof(object) ) &&
                   MessageChannels[typeof(object)].Contains(channelName));
        }

        public void AddChannelForMessageType(Type messageType, string channelName)
        {
            List<string> channels = null;
            if (!MessageChannels.TryGetValue(messageType,
                                               out channels))
            {
                channels = new List<string>();
                MessageChannels.TryAdd(messageType,
                                        channels);
            }
            if (!channels.Contains(channelName))
                channels.Add(channelName);
        }

        public void ValidateChannelDefinition(IChannelDefinition definition)
        {
            var violations = GetDefinitionViolations( definition ).ToList();
            if(violations.Count > 0)
                throw new InvalidChannelDefinitionException() { Violations = violations };
        }

        public IEnumerable<string> GetDefinitionViolations(IChannelDefinition definition)
        {
            if (definition.FactoryType == null)
                yield return "Channel definition must specify a channel factory type";

            if(definition.ChannelType == null)
                yield return "Channel definition must specify a channel type";

            if (string.IsNullOrWhiteSpace(definition.Name))
                yield return "Channel definition must specify a channel name";

            yield break;
        }

        public ChannelManager()
        {
            Channels = new ConcurrentDictionary<int, IChannel>();
            ChannelFactories = new ConcurrentDictionary<Type, IChannelFactory>();
            Definitions = new ConcurrentDictionary<int, IChannelDefinition>();
            MessageChannels = new ConcurrentDictionary<Type, List<string>>();
        }
    }
}