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
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.DI
{
    public class ScanInstruction
        : IScanInstruction
    {
        protected bool ShouldAddSingleImplementations { get; set; }
        public IList<Type> AutoWireupTypesOf { get; set; }
        public IList<Type> AutoWireupClosersOf { get; set; }
        public Func<Type, string> NamingStrategy { get; set; }
        public bool HasNamingStrategy { get { return NamingStrategy != null; } }

        public ScanInstruction()
        {
            AutoWireupTypesOf = new List<Type>();
            AutoWireupClosersOf = new List<Type>();
        }

        public void AddAllTypesOf<TPlugin>()
        {
            AutoWireupTypesOf.Add( typeof( TPlugin ) );
        }

        public void AddAllTypesOf( Type pluginType )
        {
            AutoWireupTypesOf.Add( pluginType );
        }

        public void AddSingleImplementations()
        {
            ShouldAddSingleImplementations = true;
        }

        public void Execute( IDependencyRegistry registry )
        {
            AutoWireupTypesOf
                .ForEach( x => RegisterAllTypesOf( x, registry ) );

            AutoWireupClosersOf
                .Where( x => x.IsOpenGeneric() )
                .ForEach( x => RegisterClosingTypes( x, registry ) );

            if ( ShouldAddSingleImplementations )
            {
                RegisterSingleImplementations( registry );
            }
        }

        protected void RegisterClosingTypes( Type type, IDependencyRegistry registry )
        {
            var list = Assimilate.ScanIndex.Closers.TryGet( type );
            if ( !list.Item1 )
                return;
            var matches = list.Item2.Where( x => !x.IsAbstract && !x.IsInterface );

            foreach( var match in matches )
            {
                if ( type.IsInterface )
                {
                    RegisterTypeClosingInterface( type, match, registry );
                }
                else
                {
                    RegisterClosingType( type, match, registry );
                }
            }
        }

        protected void RegisterAllTypesOf( Type type, IDependencyRegistry registry )
        {
            var list = Assimilate.ScanIndex.ImplementorsOfType.TryGet( type );

            if( list.Item1 )
                list.Item2
                    .Where( x => x.IsConcrete() )
                    .Where( x => x.GetGenericCardinality() == type.GetGenericCardinality() )
                    .ForEach( m =>
                                  {
                                      var name = HasNamingStrategy ? NamingStrategy( m ) : string.Empty;
                                      var dependencyExpression = HasNamingStrategy
                                          ? DependencyExpression.For( name, type)
                                          : DependencyExpression.For( type );
                                      dependencyExpression.Add( m );
                                      registry.Register( dependencyExpression );
                                  } );
        }

        protected void RegisterClosingType( Type type, Type match, IDependencyRegistry registry )
        {
            //Type pluginType = null;
            //Type baseType = match.BaseType;
            //while ( pluginType == null && baseType != typeof( object ) )
            //{
            //    if ( baseType != null && baseType.IsGenericType )
            //    {
            //        var genericTypeDefinition = baseType.GetGenericTypeDefinition();
            //        if ( genericTypeDefinition.Equals( type ) )
            //        {
            //            pluginType = baseType;
            //        }
            //        else
            //        {
            //            baseType = baseType.BaseType;
            //        }
            //    }
            //}
            //if ( pluginType != null )
            //{
            //    var name = HasNamingStrategy ? NamingStrategy( pluginType ) : string.Empty;
            //    var dependencyExpression = HasNamingStrategy
            //                          ? DependencyExpression.For( name, pluginType )
            //                          : DependencyExpression.For( pluginType );
            //    dependencyExpression.Use( match );
            //    registry.Register( dependencyExpression );
            //}
            var name = HasNamingStrategy ? NamingStrategy( type ) : string.Empty;
                var dependencyExpression = HasNamingStrategy
                                      ? DependencyExpression.For( name, type )
                                      : DependencyExpression.For( type );
                dependencyExpression.Use( match );
                registry.Register( dependencyExpression );
        }

        protected void RegisterTypeClosingInterface( Type type, Type match, IDependencyRegistry registry )
        {
            //var pluginTypes = match
            //    .GetInterfaces()
            //    .Where( y => y.IsGenericType && y.GetGenericTypeDefinition().Equals( type ) );

            //foreach( var pluginType in pluginTypes )
            //{
            //    var name = HasNamingStrategy ? NamingStrategy(pluginType) : string.Empty;
            //    var dependencyExpression = HasNamingStrategy
            //                          ? DependencyExpression.For( name, pluginType )
            //                          : DependencyExpression.For( pluginType);
            //    dependencyExpression.Add( match );
            //    registry.Register( dependencyExpression );
            //}

            var pluginType = type.MakeGenericType( match.GetInterfaces()
                .First( x => x.IsGenericType && x.GetGenericTypeDefinition().Equals( type ))
                .GetGenericArguments() );

            var name = HasNamingStrategy ? NamingStrategy( type ) : string.Empty;
            var dependencyExpression = HasNamingStrategy
                                    ? DependencyExpression.For( name, pluginType )
                                    : DependencyExpression.For( pluginType );
            dependencyExpression.Add( match );
            registry.Register( dependencyExpression );
        }

        protected void RegisterSingleImplementations( IDependencyRegistry registry )
        {
            //var interfaces = filteredTypes
            //    .Where( t => t.IsInterface )
            //    .Distinct();
            //var lookups = interfaces.ToDictionary(
            //    x => x,
            //    x => filteredTypes.Where( f => f.IsConcreteAndAssignableTo( x ) ) );
            //lookups
            //    .Where( kp => kp.Value.Count() == 1 )
            //    .ForEach( kp =>
            //                  {
            //                      try
            //                      {
            //                          var dependencyExpression = DependencyExpression.For( kp.Key );
            //                          dependencyExpression.Use( kp.Value.First() );
            //                          registry.Register( dependencyExpression );
            //                      }
            //                      catch ( Exception e )
            //                      {
            //                      }
            //                  } );

            Assimilate
                .ScanIndex
                .SingleImplementations
                .Where( x => x.Value.IsConcrete() )
                .ForEach( x =>
                    {
                        var dependencyExpression = DependencyExpression.For( x.Key );
                        dependencyExpression.Use( x.Value );
                        registry.Register( dependencyExpression );
                    } );
        }

        public void ConnectImplementationsToTypesClosing( Type openGenericType )
        {
            AutoWireupClosersOf.Add( openGenericType );
        }

        public void UseNamingStrategyForMultiples( Func<Type, string> strategy )
        {
            NamingStrategy = strategy;
        }
    }
}