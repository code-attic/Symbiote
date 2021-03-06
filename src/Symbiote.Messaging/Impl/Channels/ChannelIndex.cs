﻿// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Channels
{
    public class ChannelIndex
        : IChannelIndex
    {
        public ConcurrentDictionary<string, IChannelDefinition> Definitions { get; set; }

        public void AddDefinition( IChannelDefinition definition )
        {
            ValidateChannelDefinition( definition );

            Definitions.AddOrUpdate(
                definition.Name,
                definition,
                ( x, y ) => definition );
        }

        public IChannelDefinition GetDefinition( string channelName )
        {
            IChannelDefinition definition;
            if ( !Definitions.TryGetValue( channelName, out definition ) )
            {
                throw new MissingChannelDefinitionException(
                    "There was no definition provided for a channel named {0}. Please check that you have defined a channel before attempting to use it."
                        .AsFormat( channelName ) );
            }
            return definition;
        }

        public IEnumerable<IChannelDefinition> GetDefinitions()
        {
            return Definitions.Values;
        }

        public bool HasChannelFor( string channelName )
        {
            return Definitions.ContainsKey( channelName );
        }

        public IEnumerable<string> GetDefinitionViolations( IChannelDefinition definition )
        {
            if ( definition.FactoryType == null )
                yield return "Channel definition must specify a channel factory type";

            if ( definition.ChannelType == null )
                yield return "Channel definition must specify a channel type";

            if ( string.IsNullOrWhiteSpace( definition.Name ) )
                yield return "Channel definition must specify a channel name";

            yield break;
        }

        public void ValidateChannelDefinition( IChannelDefinition definition )
        {
            var violations = GetDefinitionViolations( definition ).ToList();
            if ( violations.Count > 0 )
                throw new InvalidChannelDefinitionException {Violations = violations};
        }

        public ChannelIndex()
        {
            Definitions = new ConcurrentDictionary<string, IChannelDefinition>();
        }
    }
}