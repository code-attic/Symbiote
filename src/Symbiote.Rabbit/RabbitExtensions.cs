using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging;
using Symbiote.Rabbit.Impl.Endpoint;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit
{
    public static class RabbitExtensions
    {
        public static IBus AddRabbitChannel(this IBus bus, string channelName, Action<RabbitEndpointFluentConfigurator> configurate)
        {
            IConnectionManager configuration = ServiceLocator.Current.GetInstance<IConnectionManager>();
            configuration.Endpoints.ConfigureEndpoint(configurate);
            return bus;
        }

        public static IBus AddRabbitQueue(this IBus bus, string subscription, Action<RabbitEndpointFluentConfigurator> configurate)
        {
            IConnectionManager configuration = ServiceLocator.Current.GetInstance<IConnectionManager>();
            configuration.Endpoints.ConfigureEndpoint(configurate);
            return bus;
        }
    }
}
