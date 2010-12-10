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
using System.Linq;
using Symbiote.Actor;
using Symbiote.Actor.Impl.Eventing;
using Symbiote.Actor.Impl.Saga;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Config;
using Symbiote.Messaging.Impl;
using Symbiote.Messaging.Impl.Channels;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Eventing;
using Symbiote.Messaging.Impl.Subscriptions;

namespace Symbiote.Messaging
{
    public static class MessagingAssimilation
    {
        private static IDisposable EventSubscription { get; set; }

        public static IAssimilate Messaging(this IAssimilate assimilate, Action<EventChannelConfigurator> eventChannels)
        {
            var publisher = Assimilate.GetInstanceOf<IEventPublisher>() as IObservable<IEvent>;
            if(publisher == null)
            {
                throw new AssimilationException( "You must call the Actor assimilation extension method before setting up event channels in Symbiote.Messaging." );
            }

            Messaging( assimilate );
            var configurator = new EventChannelConfigurator();
            eventChannels( configurator );
            Assimilate.Dependencies( x => x.For<IEventChannelConfiguration>().Use( configurator.Configuration ) );

            EventSubscription = publisher.Subscribe( Assimilate.GetInstanceOf<EventChannel>() );

            return assimilate;
        }

        public static IAssimilate Messaging(this IAssimilate assimilate)
        {
            assimilate.Dependencies(x =>
            {
                x.Scan(ScanAssemblies);
                DefineDependencies( x );
            } );

            var handlerInterfaces = GetHandlerInterfaces().ToList();
            var dispatcherPairs = GetMessageDispatcherPairs( handlerInterfaces );
            var actorDispatcherPairs = GetActorDispatcherPairs( handlerInterfaces );
            var sagaPairs = GetSagaDispatcherPairs();
            
            var simpleInterface = typeof(IDispatchMessage);
            Assimilate.Dependencies(x =>
                                        {
                                            dispatcherPairs.ForEach( p =>
                                            {
                                                x.For(p.Item1).Use(p.Item2);
                                                x.For(simpleInterface).Add(p.Item2);
                                            });

                                            actorDispatcherPairs.ForEach( p =>
                                            {
                                                x.For(p.Item1).Use(p.Item2);
                                                x.For(simpleInterface).Add(p.Item2);
                                            });

                                            sagaPairs.ForEach( p =>
                                            {
                                                x.For( p.Item1 ).Use( p.Item2 );
                                                x.For( simpleInterface ).Add( p.Item2 );
                                            } );
                                        });
            // Pre-load expensive instances
            Preload();

            return assimilate;
        }

        private static void DefineDependencies( DependencyConfigurator x ) 
        {
            x.For<IBus>().Use<Bus>();
            x.For<IChannelManager>().Use<ChannelManager>().AsSingleton();
            x.For<IChannelIndex>().Use<ChannelIndex>().AsSingleton();
            x.For<IDispatcher>().Use<DispatchManager>().AsSingleton();
            //x.For<IDispatcher>().Use<HyperDispatchManager>().AsSingleton();
            x.For<ISubscriptionManager>().Use<SubscriptionManager>().AsSingleton();
            
        }

        private static IEnumerable<Tuple<Type, Type>> GetSagaDispatcherPairs()
        {
            var sagas =
                Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .GetAllInstances<ISaga>();

            return sagas
                .SelectMany(s =>
                {
                    var actorType = s.ActorType;
                    return s
                        .Handles
                        .Select(x =>
                        {
                            var dispatchInterface = typeof(IDispatchToSaga<,,>).MakeGenericType(s.GetType(), actorType, x);
                            var dispatchType = typeof(SagaMessageDispatcher<,,>).MakeGenericType(s.GetType(), actorType, x);
                            return Tuple.Create(dispatchInterface, dispatchType);
                        });
                });
        }

        private static IEnumerable<Tuple<Type, Type>> GetActorDispatcherPairs(List<Type> handlers)
        {
            return handlers
                .SelectMany(h =>
                {
                    var interfaces = h.GetInterfaces().Where(t => t.Name == "IHandle`2");
                    return interfaces
                        .Select(handler =>
                        {
                            var messageType = handler.GetGenericArguments()[1];
                            var actorType = handler.GetGenericArguments()[0];
                            var dispatcherInterface =
                                typeof(IDispatchMessage<,>).MakeGenericType(actorType,
                                                                             messageType);
                            var dispatcherType =
                                typeof(ActorMessageDispatcher<,>).MakeGenericType(
                                    actorType,
                                    messageType);
                            return Tuple.Create(dispatcherInterface, dispatcherType);
                        });
                });
        }

        private static IEnumerable<Tuple<Type, Type>> GetMessageDispatcherPairs(List<Type> handlers)
        {
            return handlers
                .SelectMany(h =>
                {
                    var interfaces = h.GetInterfaces().Where(t => t.Name == "IHandle`1");
                    return interfaces
                        .Select(handler =>
                        {
                            var messageType = handler.GetGenericArguments()[0];
                            var dispatchInterface = typeof(IDispatchMessage<>).MakeGenericType(messageType);
                            var dispatchType = typeof(MessageDispatcher<>).MakeGenericType(messageType);
                            return Tuple.Create(dispatchInterface, dispatchType);
                        });
                });
        }

        private static IEnumerable<Type> GetHandlerInterfaces()
        {
            return
                Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .RegisteredPluginTypes
                    .Where(x => typeof(IHandle).IsAssignableFrom(x) || x.IsAssignableFrom(typeof(IHandle)));
        }

        private static void Preload()
        {
            Assimilate.GetInstanceOf<IDispatcher>();
        }

        private static void ScanAssemblies( IScanInstruction s )
        {
            AppDomain
                .CurrentDomain
                .GetAssemblies()
                .Where(a =>
                    a.GetReferencedAssemblies().Any(
                        r => r.FullName.Contains("Symbiote.Messaging")) ||
                    a.FullName.Contains("Symbiote.Messaging"))
                .ForEach(s.Assembly);

            s.ConnectImplementationsToTypesClosing(
                typeof (IHandle<>));
            s.ConnectImplementationsToTypesClosing(
                typeof (IHandle<,>));
        }
    }
}