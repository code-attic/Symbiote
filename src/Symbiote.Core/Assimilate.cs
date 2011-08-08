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
using System.Reflection;
using Symbiote.Core.DI;
using Symbiote.Core.DI.Impl;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;

namespace Symbiote.Core
{
    public static class Assimilate
    {
        public static IAssimilate Assimilation { get; set; }
        public static ScanIndex ScanIndex { get; set; }
        private static bool Initialized { get; set; }

        static Assimilate()
        {
            Assimilation = new Assimilation();
            ScanIndex = new ScanIndex();
            ScanIndex.Start();
        }

        public static IAssimilate Initialize()
        {
            if( Initialized )
                return Assimilation;
            
            Initialized = true;
            Wireup();
            return Assimilation;
        }

        private static void Wireup()
        {
            Type defaultAdapter = typeof( SimpleDependencyRegistry );
            var dependencyAdapterType = ScanIndex
                .ImplementorsOfType[typeof( IDependencyAdapter )]
                .FirstOrDefault( x => !x.Equals( defaultAdapter ) ) ?? defaultAdapter;
            var dependencyAdapter = Activator.CreateInstance( dependencyAdapterType ) as IDependencyAdapter;
            Assimilation.DependencyAdapter = dependencyAdapter;
            
            var initializers = new List<Type>();
            ScanIndex.ConfiguredSymbiotes.Keys.ToList().ForEach( x => LoadSymbioteDependencies( x, initializers ) );
            var uniqueInitializers = initializers
                .Distinct();

            uniqueInitializers
                .ForEach( InitializeSymbiote );

            ScanIndex
                .SymbioteInitializers
                .Except( uniqueInitializers )
                .Distinct()
                .ForEach( InitializeSymbiote );
        }

        private static void LoadSymbioteDependencies( Assembly assembly, List<Type> initializers )
        {
            if ( ScanIndex.ConfiguredSymbiotes[assembly] )
                return;

            var dependencies = GetDependencies( assembly );
            dependencies.ForEach( x => LoadSymbioteDependencies( x, initializers ) );

            var scanInstructionType = ScanIndex.ScanningInstructions.FirstOrDefault( x => x.Assembly.Equals( assembly ) );
            var dependencyDefinitionType = ScanIndex.DependencyDefinitions.FirstOrDefault( x => x.Assembly.Equals( assembly ) );
            
            var scanInstructions = scanInstructionType != null 
                ? Activator.CreateInstance( scanInstructionType ) as IProvideScan
                : null;

            var dependencyDefinitions = dependencyDefinitionType != null
                ? Activator.CreateInstance( dependencyDefinitionType ) as IDefineDependencies
                : null;

            Assimilation.Dependencies( x =>
                {
                    if( scanInstructions != null )
                        x.Scan( scanInstructions.Scan() );
                    
                    if ( dependencyDefinitions != null )
                        dependencyDefinitions.Dependencies()( x );
                } );

            ScanIndex.ConfiguredSymbiotes[ assembly ] = true;
            var initializerType = ScanIndex.SymbioteInitializers.FirstOrDefault( x => x.Assembly.Equals( assembly ) );
            if( initializerType != null )
                initializers.Add( initializerType );
        }

        private static void InitializeSymbiote( Type initializerType )
        {
            var initializer = initializerType != null
                ? Activator.CreateInstance( initializerType ) as IInitialize
                : null;

            if ( initializer != null )
                initializer.Initialize();
        }

        public static List<Assembly> GetDependencies( Assembly assembly )
        {
            return ScanIndex
                .ReferenceLookup[assembly]
                .Where( x => ScanIndex.ConfiguredSymbiotes.Keys.Any( r => r.Equals( x ) ) )
                .ToList();
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

        public static void Reset()
        {
            Initialized = false;
            Assimilation.DependencyAdapter.Reset();
        }

        public static IAssimilate UseTestLogAdapter( this IAssimilate assimilate )
        {
            assimilate.Dependencies( x => x.For<ILogProvider>().Use<TestLogProvider>().AsSingleton() );
            var logManager = GetInstanceOf<ILogManager>();
            logManager.Initialize();
            return assimilate;
        }
    }
}
