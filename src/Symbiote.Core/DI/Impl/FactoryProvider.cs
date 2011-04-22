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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Symbiote.Core.Collections;
using Symbiote.Core.Reflection;

namespace Symbiote.Core.DI.Impl
{
    public class FactoryProvider :
        IProvideInstanceFactories
    {
        public ExclusiveConcurrentDictionary<Tuple<Type, Type, string>, IProvideInstance> Providers { get; set; }

        public ConstantExpression ContainerReference { get; set; }
        public MethodInfo ResolveMethod { get; set; }
        
        public IProvideInstance GetProviderForPlugin<TRequest>( IDependencyDefinition definition, IDependencyAdapter container )
        {
            return GetProviderForPlugin( typeof( TRequest ), definition, container );
        }

        public IProvideInstance GetProviderForPlugin( Type requested, IDependencyDefinition definition, IDependencyAdapter container )
        {
            return Providers
                .ReadOrWrite( 
                    Tuple.Create( requested, definition.ConcreteType, definition.PluginName ?? ""), 
                    () => CreateProvider( requested, definition, container ) );
        }

        public IProvideInstance CreateProvider( Type requested, IDependencyDefinition definition, IDependencyAdapter container )
        {
            IProvideInstance valueProvider = null;
            if( definition.IsSingleton )
            {
                valueProvider = definition.HasSingleton
                                    ? new SingletonFactory( definition.ConcreteInstance )
                                    : new SingletonFactory( BuildFactory( requested, definition, container ) );
            }
            else
            {
                valueProvider = new InstanceFactory( BuildFactory( requested, definition, container ) );
            }
            return valueProvider;
        }

        public Func<object> BuildFactory( Type requested, IDependencyDefinition definition, IDependencyAdapter container )
        {
            ContainerReference = ContainerReference ?? Expression.Constant( container );
            ResolveMethod = ResolveMethod ?? container.GetType().GetMethod( "GetInstance", new Type[] { typeof( Type ) } );

            var genericTypeArgs = requested.IsGenericType
                                      ? requested.GetGenericArguments()
                                      : new Type[] {};

            var type = genericTypeArgs.Length > 0 && definition.ConcreteType.IsGenericTypeDefinition
                           ? definition.ConcreteType.MakeGenericType( genericTypeArgs )
                           : definition.ConcreteType;

            var constructors = Reflector.GetConstructorInfo( type ).OrderBy( x => x.Item2.Length ).Reverse();
            var constructor = constructors
                .FirstOrDefault( x => x.Item2.All( p => container.HasPluginFor( p.ParameterType ) ) );

            var parameters = constructor
                .Item2
                .Select( x =>
                    {
                        var argExpr = Expression.Constant( x.ParameterType );
                        var callExpr = Expression.Call( ContainerReference, ResolveMethod, argExpr );
                        return Expression.Convert( callExpr, x.ParameterType );
                    } );

            var newExpr = Expression.New( constructor.Item1, parameters );
            var castExpr = Expression.Convert( newExpr, typeof( object ) );
            return Expression.Lambda<Func<object>>( castExpr ).Compile();
        }

        public FactoryProvider()
        {
            Providers = new ExclusiveConcurrentDictionary<Tuple<Type, Type, string>, IProvideInstance>();
        }
    }
}