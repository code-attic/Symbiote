using System;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Messaging.Impl.Dispatch;
using Symbiote.Messaging.Impl.Saga;

namespace Symbiote.Messaging.Config
{
    public class MessagingInitializer : IInitializeSymbiote
    {
        public void Initialize()
        {
            var handlerInterfaces = GetHandlerInterfaces().ToList();
            var dispatcherPairs = GetMessageDispatcherPairs( handlerInterfaces );
            var actorDispatcherPairs = GetActorDispatcherPairs( handlerInterfaces );
            var sagaPairs = GetSagaDispatcherPairs();

            var simpleInterface = typeof( IDispatchMessage );
            Assimilate.Dependencies( x =>
                                         {
                                             dispatcherPairs
                                                 .ForEach( p =>
                                                               {
                                                                   x.For( p.Item1 ).Use( p.Item2 );
                                                                   x.For( simpleInterface ).Add( p.Item2 );
                                                               } );

                                             actorDispatcherPairs
                                                 .ForEach( p =>
                                                               {
                                                                   x.For( p.Item1 ).Use( p.Item2 );
                                                                   x.For( simpleInterface ).Add( p.Item2 );
                                                               } );

                                             sagaPairs
                                                 .ForEach( p =>
                                                               {
                                                                   x.For( p.Item1 ).Use( p.Item2 );
                                                                   x.For( simpleInterface ).Add( p.Item2 );
                                                               } );
                                         } );
        }

        private IEnumerable<Tuple<Type, Type>> GetSagaDispatcherPairs()
        {
            var sagas =
                Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .GetAllInstances<ISaga>();

            return sagas
                .SelectMany( s =>
                                 {
                                     var actorType = s.ActorType;
                                     return s
                                         .Handles
                                         .Select( x =>
                                                      {
                                                          var dispatchInterface =
                                                              typeof( IDispatchToSaga<,,> ).MakeGenericType(
                                                                  s.GetType(), actorType, x );
                                                          var dispatchType =
                                                              typeof( SagaMessageDispatcher<,,> ).MakeGenericType(
                                                                  s.GetType(), actorType, x );
                                                          return Tuple.Create( dispatchInterface, dispatchType );
                                                      } );
                                 } );
        }

        private IEnumerable<Tuple<Type, Type>> GetActorDispatcherPairs( List<Type> handlers )
        {
            return handlers
                .SelectMany( h =>
                                 {
                                     var interfaces = h.GetInterfaces().Where( t => t.Name == "IHandle`2" );
                                     return interfaces
                                         .Select( handler =>
                                                      {
                                                          var messageType = handler.GetGenericArguments()[1];
                                                          var actorType = handler.GetGenericArguments()[0];
                                                          var dispatcherInterface =
                                                              typeof( IDispatchMessage<,> ).MakeGenericType( actorType,
                                                                                                             messageType );
                                                          var dispatcherType =
                                                              typeof( ActorMessageDispatcher<,> ).MakeGenericType(
                                                                  actorType,
                                                                  messageType );
                                                          return Tuple.Create( dispatcherInterface, dispatcherType );
                                                      } );
                                 } );
        }

        private IEnumerable<Tuple<Type, Type>> GetMessageDispatcherPairs( List<Type> handlers )
        {
            return handlers
                .SelectMany( h =>
                                 {
                                     var interfaces = h.GetInterfaces().Where( t => t.Name == "IHandle`1" );
                                     return interfaces
                                         .Select( handler =>
                                                      {
                                                          var messageType = handler.GetGenericArguments()[0];
                                                          var dispatchInterface =
                                                              typeof( IDispatchMessage<> ).MakeGenericType( messageType );
                                                          var dispatchType =
                                                              typeof( MessageDispatcher<> ).MakeGenericType( messageType );
                                                          return Tuple.Create( dispatchInterface, dispatchType );
                                                      } );
                                 } );
        }

        private IEnumerable<Type> GetHandlerInterfaces()
        {
            return
                Assimilate
                    .Assimilation
                    .DependencyAdapter
                    .RegisteredPluginTypes
                    .Where( x => typeof( IHandle ).IsAssignableFrom( x ) || x.IsAssignableFrom( typeof( IHandle ) ) );
        }
    }
}