using System;
using System.Collections.Generic;
using Symbiote.Jackalope.Impl;
using StructureMap;
using Symbiote.Jackalope.Impl.Serialization;

namespace Symbiote.Jackalope.Config
{
    public class AmqpFluentConfiguration : IAmqpConfigurationProvider
    {
        public List<string> MessageSubscriptions { get; set; }

        public IList<IAmqpServerConfiguration> Servers
        {
            get;
            set;
        }

        public AmqpFluentConfiguration AddServer(Action<AmqpFluentServerConfiguration> configure)
        {
            var fluentServerConfiguration = new AmqpFluentServerConfiguration();
            configure(fluentServerConfiguration);
            Servers.Add(fluentServerConfiguration.Server);
            return this;
        }

        public AmqpFluentConfiguration SerializeMessagesAsBinary()
        {
            ObjectFactory.Configure(c => c.For<IMessageSerializer>().Use<NetBinarySerializer>());
            return this;
        }

        public AmqpFluentConfiguration()
        {
            Servers = new List<IAmqpServerConfiguration>();
            MessageSubscriptions = new List<string>();
        }
    }
}