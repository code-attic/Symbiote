using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Subscriptions;
using Symbiote.Rabbit.Impl.Adapter;
using Symbiote.Rabbit.Impl.Endpoint;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit
{
    public static class RabbitExtensions
    {
        public static IBus AddRabbitChannel(this IBus bus, string channelName, Action<RabbitEndpointFluentConfigurator> configurate)
        {
            IEndpointManager endpoints = Assimilate.GetInstanceOf<IEndpointManager>();
            endpoints.ConfigureEndpoint(configurate);
            return bus;
        }

        public static IBus AddRabbitQueue(this IBus bus, string subscription, Action<RabbitEndpointFluentConfigurator> configurate)
        {
            IEndpointManager endpoints = Assimilate.GetInstanceOf<IEndpointManager>();
            ISubscriptionManager subscriptions = Assimilate.GetInstanceOf<ISubscriptionManager>();
            endpoints.ConfigureEndpoint(configurate);
            var queueSubscription = Assimilate.GetInstanceOf<QueueSubscription>();
            queueSubscription.Name = subscription;
            subscriptions.AddSubscription(queueSubscription);
            return bus;
        }
    }
}
