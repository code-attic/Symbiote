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
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit
{
    public static class RabbitExtensions
    {
        public static IBus AddRabbitQueue(this IBus bus, Action<EndpointConfigurator> configurate)
        {
            var endpoints = Assimilate.GetInstanceOf<IEndpointManager>();
            endpoints.ConfigureEndpoint(configurate);
            return bus;
        }

        public static IBus AddRabbitChannel(this IBus bus, Action<ChannelConfigurator> configurate)
        {
            var channels = Assimilate.GetInstanceOf<IChannelIndex>();
            var connectionManager = Assimilate.GetInstanceOf<IConnectionManager>();
            var configurator = new ChannelConfigurator();
            
            configurate( configurator );
            var channelDefinition = configurator.ChannelDefinition;
            
            channelDefinition.CreateOnBroker( connectionManager );
            channels.AddDefinition( channelDefinition );
            return bus;
        }

        public static void CommitChannelOf(this IBus bus, string channelName)
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor(channelName) as IHaveChannelProxy;
            channel.Proxy.Channel.TxCommit();
        }

        public static void RollbackChannelOf(this IBus bus, string channelName)
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor(channelName) as IHaveChannelProxy;
            channel.Proxy.Channel.TxRollback();
        }
    }
}
