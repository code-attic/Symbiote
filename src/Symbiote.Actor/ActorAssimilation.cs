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
using Symbiote.Actor.Impl.Actor;
using Symbiote.Actor.Impl.Actor.Defaults;
using Symbiote.Actor.Impl.Memento;
using Symbiote.Core;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Actor
{
    public static class ActorAssimilation
    {
        public static IAssimilate Actors(this IAssimilate assimilation)
        {
            assimilation.Dependencies( x =>
            {
                x.For<IAgency>().Use<Agency>().AsSingleton();

                x.For(typeof(IActorCache<>)).Use(typeof(NullActorCache<>));
                x.For(typeof(IAgentFactory)).Use<DefaultAgentFactory>();
                x.For(typeof(KeyAccessAdapter<>)).Use(typeof(KeyAccessAdapter<>));
                x.For(typeof(IKeyAccessor<>)).Use(typeof(DefaultKeyAccessor<>));
                x.For(typeof(IAgent<>)).Use(typeof(DefaultAgent<>));
                x.For(typeof(IActorStore<>)).Use(typeof(NullActorStore<>));
                x.For(typeof(IActorFactory<>)).Use(typeof(DefaultActorFactory<>));
                x.For<IMemoizer>().Use<Memoizer>();
                x.For( typeof(IMemento<>) ).Use( typeof(ReflectiveMemento<>) );

                x.Scan( s =>
                {
                    AppDomain
                        .CurrentDomain
                        .GetAssemblies()
                        .Where(a =>
                            a.GetReferencedAssemblies().Any(
                                r => r.FullName.Contains("Symbiote.Actor")) ||
                            a.FullName.Contains("Symbiote.Actor"))
                        .ForEach(s.Assembly);

                    s.AddAllTypesOf<ISaga>();
                    s.ConnectImplementationsToTypesClosing(
                        typeof(IActorFactory<>));
                    s.ConnectImplementationsToTypesClosing(
                        typeof(IKeyAccessor<>));
                    s.ConnectImplementationsToTypesClosing(
                        typeof(ISaga<>));
                } );
            } );

            Preload();
            return assimilation;
        }

        private static IEnumerable<Type> GetKnownActorTypes()
        {
            var keyAccessorTypes = Assimilate.Assimilation.DependencyAdapter.RegisteredPluginTypes.Where( x => x.IsAssignableFrom( typeof(IKeyAccessor) ) );

            foreach (var keyAccessorType in keyAccessorTypes)
            {
                var type = keyAccessorType.GetInterface( "IKeyAccessor`1" ).GetGenericArguments().FirstOrDefault();
                if (type != null)
                    yield return type;
            }

            var actorFactoryTypes = Assimilate.Assimilation.DependencyAdapter.RegisteredPluginTypes.Where( x => 
                x.IsAssignableFrom( typeof(IActorFactory)) ||
                typeof(IActorFactory).IsAssignableFrom( x ));

            foreach (var actorFactoryType in actorFactoryTypes)
            {
                var type = actorFactoryType.GetInterface("IActorFactory`1").GetGenericArguments().FirstOrDefault();
                if (type != null && !string.IsNullOrWhiteSpace(type.FullName))
                    yield return type;
            }
        }

        private static void Preload()
        {
            var agency = Assimilate.GetInstanceOf<IAgency>();

            foreach (var actorType in GetKnownActorTypes())
            {
                agency.GetAgentFor( actorType );
            }
        }
    }
}
