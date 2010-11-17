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
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Channels
{
    public class ChannelIndex
        : IChannelIndex
    {
        public ConcurrentDictionary<int, IChannelDefinition> Definitions { get; set; }
        public ConcurrentDictionary<Type, List<string>> MessageChannels { get; set; }

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

        public void AddDefinition(IChannelDefinition definition)
        {
            ValidateChannelDefinition(definition);

            Definitions.AddOrUpdate(
                GetChannelKey(definition.MessageType, definition.Name),
                definition,
                (x, y) => definition);

            AddChannelForMessageType(definition.MessageType,
                                      definition.Name);
        }

        public int GetChannelKey(Type messageType, string name)
        {
            return (name.GetHashCode() * 397) ^ messageType.GetHashCode();
        }

        public IEnumerable<IChannelDefinition> GetDefinitionsFor<TMessage>()
        {
            return GetKeysFor<TMessage>().Select( x => Definitions[x] );
        }

        public IChannelDefinition GetDefinitionFor<TMessage>( string channelName )
        {
            IChannelDefinition definition = null;
            if (!Definitions.TryGetValue( GetKeyFor<TMessage>( channelName ), out definition ))
            {
                throw new MissingChannelDefinitionException(
                    "There was no definition provided for a channel named {0} of message type {1}. Please check that you have defined a channel before attempting to use it."
                        .AsFormat(channelName, typeof(TMessage)));
            }
            return definition;
        }

        public IEnumerable<string> GetDefinitionViolations(IChannelDefinition definition)
        {
            if (definition.FactoryType == null)
                yield return "Channel definition must specify a channel factory type";

            if (definition.ChannelType == null)
                yield return "Channel definition must specify a channel type";

            if (string.IsNullOrWhiteSpace(definition.Name))
                yield return "Channel definition must specify a channel name";

            yield break;
        }

        public int GetKeyFor<TMessage>(string channelName)
        {
            var typedKey = GetChannelKey( typeof(TMessage), channelName );
            if (Definitions.ContainsKey(typedKey))
                return typedKey;
            else
                return GetChannelKey( typeof(object), channelName );
        }

        public IEnumerable<int> GetKeysFor<TMessage>()
        {
            List<string> channelNameList;
            var messageType = typeof(TMessage);
            
            if (MessageChannels.TryGetValue(messageType, out channelNameList))
            {
                return channelNameList.Select( x => GetChannelKey( messageType, x ) );
            }

            var objectType = typeof(object);
            if(MessageChannels.TryGetValue(objectType, out channelNameList))
            {
                return channelNameList.Select( x => GetChannelKey( objectType, x ) );    
            }

            return new List<int>();
        }

        public bool HasChannelFor<TMessage>()
        {
            return MessageChannels.ContainsKey(typeof(TMessage)) || MessageChannels.ContainsKey(typeof(object));
        }

        public bool HasChannelFor<TMessage>(string channelName)
        {
            return (MessageChannels.ContainsKey(typeof(TMessage)) &&
                   MessageChannels[typeof(TMessage)].Contains(channelName)) ||
                   (MessageChannels.ContainsKey(typeof(object)) &&
                   MessageChannels[typeof(object)].Contains(channelName));
        }

        public void ValidateChannelDefinition(IChannelDefinition definition)
        {
            var violations = GetDefinitionViolations(definition).ToList();
            if (violations.Count > 0)
                throw new InvalidChannelDefinitionException() { Violations = violations };
        }

        public ChannelIndex()
        {
            Definitions = new ConcurrentDictionary<int, IChannelDefinition>();
            MessageChannels = new ConcurrentDictionary<Type, List<string>>();
        }
    }
}
