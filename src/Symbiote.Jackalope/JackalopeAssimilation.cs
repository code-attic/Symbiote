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

            if (configuration.Brokers.Values.First().Protocol == "AMQP_0_9_1")
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
                .SelectMany(h =>
                            {
                                var interfaces = h.GetInterfaces().Where(t => t.Name == "IMessageHandler`1");
                                return interfaces
                                    .Select(handler =>
                                                {
                                                    var messageType = handler.GetGenericArguments()[0];
                                                    var dispatchInterface = typeof(IDispatch<>).MakeGenericType(messageType);
                                                    var dispatchType = typeof(Dispatcher<>).MakeGenericType(messageType);
                                                    return Tuple.Create(dispatchInterface, dispatchType);
                                                });
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
