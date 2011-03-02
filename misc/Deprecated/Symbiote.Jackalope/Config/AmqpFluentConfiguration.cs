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
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Symbiote.Jackalope.Impl;
using Symbiote.Jackalope.Impl.Serialization;

namespace Symbiote.Jackalope.Config
{
    public class AmqpFluentConfiguration : IAmqpConfigurationProvider
    {
        public IDictionary<string, IBroker> Brokers { get; set; }

        public AmqpFluentConfiguration AddServer(Action<AmqpFluentServerConfiguration> configure)
        {
            var fluentServerConfiguration = new AmqpFluentServerConfiguration();
            configure(fluentServerConfiguration);

            var amqpServer = fluentServerConfiguration.Server;

            IBroker broker = null;
            if(!Brokers.TryGetValue(amqpServer.Broker, out broker))
            {
                broker = new AmqpBroker(amqpServer.Broker);
                Brokers[broker.Name] = broker;
            }
            broker.AddServer(amqpServer);

            return this;
        }

        public AmqpFluentConfiguration SerializeMessagesAsBinary()
        {
            Assimilate.Dependencies(c => c.For<IMessageSerializer>().Use<NetBinarySerializer>());
            return this;
        }

        public AmqpFluentConfiguration()
        {
            Brokers = new Dictionary<string, IBroker>();
        }
    }
}