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