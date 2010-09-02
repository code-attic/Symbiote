using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Symbiote.Core;
using Symbiote.Jackalope.Config;
using Symbiote.Jackalope.Impl;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Impl.Channel;
using Symbiote.Jackalope.Impl.Dispatch;
using Symbiote.Jackalope.Impl.Endpoint;
using Symbiote.Jackalope.Impl.Routes;
using Symbiote.Jackalope.Impl.Serialization;
using Symbiote.Jackalope.Impl.Server;
using Symbiote.Jackalope.Impl.Subscriptions;
using Tuple = System.Tuple;

namespace Symbiote.Jackalope
{
    public static class JackalopeAssimilation
    {
        public static IAssimilate Jackalope(this IAssimilate assimilate)
        {
            ConfigureStandardDependencies();

            assimilate
                .Dependencies(x => x.For<IAmqpConfigurationProvider>()
                                       .Use<AmqpConfiguration>());
            assimilate
                .Dependencies(x => x.For<IConnectionManager>()
                                       .Use<ConnectionManager>()
                                       .AsSingleton());

            return assimilate;
        }

        public static IAssimilate Jackalope(this IAssimilate assimilate, Action<AmqpFluentConfiguration> configure)
        {
            var configuration = new AmqpFluentConfiguration();
            configure(configuration);
            ConfigureStandardDependencies();

            assimilate
                .Dependencies(x => x.For<IAmqpConfigurationProvider>()
                                       .Use(configuration));
            assimilate
                .Dependencies(x => x.For<IConnectionManager>()
                                       .Use<ConnectionManager>()
                                       .AsSingleton());

            if (configuration.Servers[0].Protocol == "AMQP_0_9_1")
                assimilate.Dependencies(x => x.For<IQueueObserver>().Use<ObservableConsumer>());

            return assimilate;
        }

        public static IAssimilate ScanForHandlersInAssemblyContaining<T>(this IAssimilate assimilate)
        {
            assimilate
                .Dependencies(x => 
                    x.Scan(s =>
                      {
                          s.AssemblyContainingType<T>();
                          s.ConnectImplementationsToTypesClosing(typeof(IMessageHandler<>));
                      }));
            return assimilate;
        }

        public static void ConfigureStandardDependencies()
        {
            Assimilate.Dependencies(x =>
                    {
                        x.For<IBus>()
                            .Use<Bus>();
                        x.For<IQueueObserver>()
                            .Use<ObservableQueue>();
                        x.For<IMessageSerializer>()
                            .Use<JsonMessageSerializer>();
                        x.For<IChannelProxyFactory>()
                            .Use<ChannelProxyFactory>();
                        x.For<IEndpointIndex>()
                            .Use(new EndpointIndex());
                        x.For<IEndpointManager>()
                            .Use<EndpointManager>();
                        x.For<IConnectionManager>()
                            .Use<ConnectionManager>();
                        x.For<IRouteManager>()
                            .Use<RouteManager>()
                            .AsSingleton();
                        x.For<ISubscription>()
                            .Use<Subscription>();
                        x.For<ISubscriptionManager>()
                            .Use(new SubscriptionManager());
                        x.For<IDispatchMessages>()
                            .Use<DispatchingObserver>()
                            .AsSingleton();
                        x.Scan(s =>
                                   {
                                       AppDomain
                                           .CurrentDomain
                                           .GetAssemblies()
                                           .Where(a => a.GetReferencedAssemblies().Any(r => r.FullName.Contains("Jackalope")))
                                           .ForEach(s.Assembly);
                                       s.ConnectImplementationsToTypesClosing(
                                           typeof (IMessageHandler<>));
                                   });
                        x.For<Action<IModel, BasicReturnEventArgs>>()
                            .Use<Action<IModel, BasicReturnEventArgs>>((m, e) =>
                                     {
                                         var message = "Message delivery to exchange {0} with routing key {1} failed: \r\n\t Reason Code {2} : {3}"
                                             .AsFormat(e.Exchange, e.RoutingKey, e.ReplyText, e.ReplyCode, e.ReplyText);
                                         throw new Exception(message);
                                     });
                    });
            var handlers =
                Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .RegisteredPluginTypes
                    .Where(x => typeof (IMessageHandler).IsAssignableFrom(x) || x.IsAssignableFrom(typeof (IMessageHandler)));

            var dispatcherPairs = handlers
                .Select(h =>
                            {
                                var handler = h.GetInterface("IMessageHandler`1");
                                 var messageType = handler.GetGenericArguments()[0];
                                 var dispatchInterface = typeof (IDispatch<>).MakeGenericType(messageType);
                                 var dispatchType = typeof (Dispatcher<>).MakeGenericType(messageType);
                                 return Tuple.Create(dispatchInterface, dispatchType);
                             });
            var simpleInterface = typeof (IDispatch);
            Assimilate.Dependencies(x => dispatcherPairs.ForEach(p =>
            {
                x.For(p.Item1).Use(p.Item2);
                x.For(simpleInterface).Add(p.Item2);
            }));
        }

    }
}
