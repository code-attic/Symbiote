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

namespace Symbiote.Rabbit
{
    public static class RabbitExtensions
    {
        public static IBus AddRabbitChannel<TMessage>(this IBus bus, Action<RabbitEndpointFluentConfigurator> configurate)
            where TMessage : class
        {
            var endpoints = Assimilate.GetInstanceOf<IEndpointManager>();
            endpoints.ConfigureEndpoint<TMessage>(configurate);
            return bus;
        }

        public static void CommitChannelOf<TMessage>(this IBus bus)
            where TMessage : class
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor<TMessage>() as RabbitChannel<TMessage>;
            channel.Proxy.Channel.TxCommit();
        }

        public static void RollbackChannelOf<TMessage>(this IBus bus)
            where TMessage : class
        {
            var channels = Assimilate.GetInstanceOf<IChannelManager>();
            var channel = channels.GetChannelFor<TMessage>() as RabbitChannel<TMessage>;
            channel.Proxy.Channel.TxRollback();
        }
    }
}
