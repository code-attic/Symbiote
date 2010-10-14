using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Rabbit.Config;
using Symbiote.Rabbit.Impl.Channels;
using Symbiote.Rabbit.Impl.Endpoint;
using Symbiote.Rabbit.Impl.Server;

namespace Symbiote.Rabbit
{
    public static class RabbitAssimilation
    {
        public static IAssimilate Rabbit(this IAssimilate assimilate, Action<RabbitConfiguration> configurate)
        {
            var configuration = new RabbitConfiguration();
            configurate(configuration);
            ConfigureStandardDependencies(assimilate, configuration);
            return assimilate;
        }

        private static void ConfigureStandardDependencies(IAssimilate assimilate, RabbitConfiguration configuration)
        {
            Assimilate.Dependencies(x =>
                                        {
                                            x.For<IConnectionManager>()
                                                .Use<ConnectionManager>()
                                                .AsSingleton();
                                            x.For<RabbitConfiguration>()
                                                .Use(configuration);
                                            x.For<IChannelProxyFactory>()
                                                .Use<ChannelProxyFactory>();
                                            x.For<IEndpointManager>()
                                                .Use<EndpointManager>();
                                            x.For<IConnectionManager>()
                                                .Use<ConnectionManager>();
                                            x.For<IEndpointIndex>()
                                                .Use(new EndpointIndex());
                                            //x.For<ISubscription>()
                                            //    .Use<Subscription>();
                                            //x.For<ISubscriptionManager>()
                                            //    .Use(new SubscriptionManager());
                                        });
        }
    }
}
