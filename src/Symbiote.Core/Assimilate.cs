// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Symbiote.Core.Actor;
using Symbiote.Core.DI;
using Symbiote.Core.Locking;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;
using Symbiote.Core.Memento;
using Symbiote.Core.Saga;
using Symbiote.Core.Serialization;
using Symbiote.Core.UnitOfWork;

namespace Symbiote.Core
{
    public static class Assimilate
    {
        public static IAssimilate Assimilation { get; set; }
        private static List<string> _assimilated = new List<string>();

#if !SILVERLIGHT
        private static ReaderWriterLockSlim _assimilationLock = new ReaderWriterLockSlim();
#endif

#if SILVERLIGHT
        private static object _assimilationLock = new object();
#endif

        private static bool Initialized { get; set; }

        static Assimilate()
        {
            Assimilation = new Assimilation();
        }

        public static IAssimilate Core<TDepedencyAdapter>()
            where TDepedencyAdapter : class, IDependencyAdapter, new()
        {
            var adapter = Activator.CreateInstance<TDepedencyAdapter>();
            return Core( adapter );
        }

        public static IAssimilate Core( IDependencyAdapter adapter )
        {
            if ( Initialized )
                return Assimilation;
            Initialized = true;
            Assimilation.DependencyAdapter = adapter;
            Assimilation.Dependencies( x =>
                                           {
                                               DefineDependencies( x );
                                               x.Scan( DefineScan );
                                           } );
            return Assimilation;
        }

        private static void DefineDependencies( DependencyConfigurator container )
        {
            container.For<ILogProvider>().Use<NullLogProvider>();
            container.For<ILogger>().Use<NullLogger>();
            container.For( typeof( ILogger<> ) ).Add( typeof( ProxyLogger<> ) );
            container.For<ILockManager>().Use<NullLockManager>();
            container.For( typeof( KeyAccessAdapter<> ) ).Use( typeof( KeyAccessAdapter<> ) );
            container.For( typeof( IKeyAccessor<> ) ).Use( typeof( DefaultKeyAccessor<> ) );
            container.For<IMemoizer>().Use<Memoizer>();
            container.For( typeof( IMemento<> ) ).Use( typeof( PassthroughMemento<> ) );
            container.For<IEventPublisher>().Use<EventPublisher>().AsSingleton();
            container.For<IContextProvider>().Use<DefaultContextProvider>();
            container.For<IEventConfiguration>().Use<EventConfiguration>();
            container.For<IJsonSerializerFactory>().Use<JsonSerializerFactory>().AsSingleton();
            container.For<IDependencyAdapter>().Use( Assimilation.DependencyAdapter );
            container.For<IEventListenerManager>().Use<EventListenerManager>().AsSingleton();
            container.For<IAgency>().Use<Agency>().AsSingleton();
            container.For(typeof(IActorCache<>)).Use(typeof(NullActorCache<>));
            container.For(typeof(IAgentFactory)).Use<DefaultAgentFactory>();
            container.For(typeof(IAgent<>)).Use(typeof(DefaultAgent<>));
            container.For(typeof(IActorStore<>)).Use(typeof(NullActorStore<>));
            container.For(typeof(IActorFactory<>)).Use(typeof(DefaultActorFactory<>));
        }

        public static void DefineScan( IScanInstruction scan )
        {
            {
                scan.AssembliesFromApplicationBaseDirectory(
                    a => { return a.GetReferencedAssemblies().Any( r => r.Name.Contains( "Symbiote" ) ); } );
                scan.AddAllTypesOf<IContractResolverStrategy>();
                scan.ConnectImplementationsToTypesClosing( typeof( IKeyAccessor<> ) );
                scan.ConnectImplementationsToTypesClosing( typeof( IMemento<> ) );
                scan.AddAllTypesOf<IEventListener>();
                scan.AddSingleImplementations();
                scan.AddAllTypesOf<ISaga>();
                scan.ConnectImplementationsToTypesClosing(
                    typeof( IActorFactory<> ) );
                scan.ConnectImplementationsToTypesClosing(
                    typeof( IActorCache<> ) );
                scan.ConnectImplementationsToTypesClosing(
                    typeof( IActorStore<> ) );
                scan.ConnectImplementationsToTypesClosing(
                    typeof( ISaga<> ) );
            }
        }

        public static IEnumerable<T> GetAllInstancesOf<T>()
        {
            return Assimilation.DependencyAdapter.GetAllInstances<T>();
        }

        public static IEnumerable GetAllInstancesOf( Type type )
        {
            return Assimilation.DependencyAdapter.GetAllInstances( type );
        }

        public static T GetInstanceOf<T>()
        {
            return Assimilation.DependencyAdapter.GetInstance<T>();
        }

        public static T GetInstanceOf<T>( string name )
        {
            return Assimilation.DependencyAdapter.GetInstance<T>( name );
        }

        public static object GetInstanceOf( Type type )
        {
            return Assimilation.DependencyAdapter.GetInstance( type );
        }

        public static object GetInstanceOf( Type type, string name )
        {
            return Assimilation.DependencyAdapter.GetInstance( type, name );
        }

        public static IAssimilate Dependencies( this IAssimilate assimilate, Action<DependencyConfigurator> configurator )
        {
            var config = new DependencyConfigurator();
            configurator( config );
            config.RegisterDependencies( assimilate.DependencyAdapter );
            return assimilate;
        }

        public static IAssimilate Dependencies( Action<DependencyConfigurator> configurator )
        {
            var config = new DependencyConfigurator();
            configurator( config );
            config.RegisterDependencies( Assimilation.DependencyAdapter );
            return Assimilation;
        }

        public static void Require( string prerequisite, Exception exception )
        {
#if !SILVERLIGHT
            _assimilationLock.EnterReadLock();
#endif
#if SILVERLIGHT
        lock(_assimilationLock)
        {
#endif
            try
            {
                if ( !_assimilated.Contains( prerequisite ) )
                {
                    throw exception;
                }
            }
            finally
            {
#if !SILVERLIGHT
                _assimilationLock.ExitReadLock();
#endif
            }
#if SILVERLIGHT
        }
#endif
        }

        public static void RegisterSymbioteLibrary( string sliverName )
        {
#if !SILVERLIGHT
            _assimilationLock.EnterWriteLock();
#endif
#if SILVERLIGHT
        lock(_assimilationLock)
        {
#endif
            try
            {
                if ( !_assimilated.Contains( sliverName ) )
                    _assimilated.Add( sliverName );
            }
            finally
            {
#if !SILVERLIGHT
                _assimilationLock.ExitWriteLock();
#endif
            }
#if SILVERLIGHT
        }
#endif
        }

        public static IAssimilate UseTestLogAdapter( this IAssimilate assimilate )
        {
            assimilate.Dependencies( x => { x.For<ILogProvider>().Use<TestLogProvider>().AsSingleton(); } );
            LogManager.Initialized = true;
            return assimilate;
        }
    }
}