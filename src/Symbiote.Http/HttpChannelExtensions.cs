// /* 
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
using System;
using Symbiote.Core;
using Symbiote.Http.Impl.Adapter.Channel;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;

namespace Symbiote.Http
{
    public static class HttpChannelExtensions
    {
        public static IBus AddHttpChannel( this IBus bus, Action<ChannelConfigurator> configurate )
        {
            var channels = Assimilate.GetInstanceOf<IChannelIndex>();
            var configurator = new ChannelConfigurator();
            configurate( configurator );
            var channelDefinition = configurator.ChannelDefinition;
            channels.AddDefinition( channelDefinition );
            return bus;
        }
    }
}