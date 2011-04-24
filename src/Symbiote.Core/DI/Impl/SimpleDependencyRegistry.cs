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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Symbiote.Core.DI.Impl
{
    public class SimpleDependencyRegistry :
        IDependencyAdapter
    {
        private Func<IDependencyDefinition, IDependencyDefinition, bool> IsDuplicateAdd = 
            ( x, y ) => x.ConcreteType.Equals( y.ConcreteType ) && x.PluginName == y.PluginName;

        private Func<IDependencyDefinition, IDependencyDefinition, bool> IsDuplicateFor = 
            ( x, y ) => x.PluginName == y.PluginName;

        public ConcurrentDictionary<Type, List<IDependencyDefinition>> Definitions { get; set; }
        public IProvideInstanceFactories Providers { get; set; }
        

        public IEnumerable<Type> RegisteredPluginTypes
        {
            get
            {
                return Definitions
                    .SelectMany( x => x
                                          .Value
                                          .Where( d => d.CreatorDelegate == null )
                                          .Select( d => d.ConcreteType ?? d.ConcreteInstance.GetType() ) );
            }
        }

        public Type GetDefaultTypeFor<T>()
        {
            List<IDependencyDefinition> definitions;
            if( Definitions.TryGetValue( typeof(T), out definitions ) )
            {
                return definitions
                    .Where( x => x.CreatorDelegate == null && !x.IsNamed )
                    .Select( x => x.ConcreteType ?? x.ConcreteInstance.GetType() )
                    .FirstOrDefault();
            }
            return null;
        }

        public IEnumerable<Type> GetTypesRegisteredFor<T>()
        {
            return GetTypesRegisteredFor( typeof( T ) );
        }

        public IEnumerable<Type> GetTypesRegisteredFor( Type type )
        {
            List<IDependencyDefinition> definitions;
            if( Definitions.TryGetValue( type, out definitions ) )
            {
                return definitions
                    .Where( x => x.CreatorDelegate == null )
                    .Select( x => x.ConcreteType ?? x.ConcreteInstance.GetType() );
            }
            return null;
        }

        public bool HasPluginFor<T>()
        {
            return HasPluginFor( typeof( T ) );
        }

        public bool HasPluginFor( Type type )
        {
            return type.IsConcrete() || 
                    Definitions.ContainsKey( type ) ||
                    Definitions.ContainsKey( type.GetGenericTypeDefinition() );
        }

        /// <summary>
        /// If the dependency is a For
        ///     AND the plugin name matches
        /// Else If the dependency is an Add
        ///     AND the concrete type matches
        /// </summary>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public bool IsDuplicate( IDependencyDefinition dependency )
        {
            var definitions = new List<IDependencyDefinition>();
            var duplicate = false;
            var predicate = dependency.IsAdd
                            ? IsDuplicateAdd
                            : IsDuplicateFor;
            if( Definitions.TryGetValue( dependency.PluginType, out definitions ) )
            {
                duplicate = definitions
                    .Any( x => predicate(x, dependency) );
            }
            return duplicate;
        }

        public void Register( IDependencyDefinition dependency )
        {
            var key = dependency.PluginType.IsGenericType && dependency.ConcreteType.IsGenericType
                          ? dependency.PluginType.GetGenericTypeDefinition()
                          : dependency.PluginType;
            Definitions
                .AddOrUpdate( key,
                              x => new List<IDependencyDefinition>() { dependency },
                              ( x, y ) =>
                                  {
                                      if( IsDuplicate( dependency )  )
                                        y.RemoveAll( d => d.PluginName == dependency.PluginName );
                                      y.Add( dependency );
                                      return y;
                                  } );
        }

        public void Reset()
        {
            Definitions = new ConcurrentDictionary<Type, List<IDependencyDefinition>>();
        }

        public void Scan( IScanInstruction scanInstruction )
        {
            scanInstruction.Execute( this );
        }

        public IDependencyDefinition GetDefinitionFor( Type type, string key )
        {
            var definitions = new List<IDependencyDefinition>();
            if( !Definitions.TryGetValue( type, out definitions ) && type.IsGenericType )
                Definitions.TryGetValue( type.GetGenericTypeDefinition(), out definitions );
            
            if( definitions == null && type.IsConcrete() )
            {
                var pluginType = type.IsGenericType
                                     ? type.GetGenericTypeDefinition()
                                     : type;
                var definition = DependencyExpression.For( pluginType );
                definition.Use( type );
                definitions = new List<IDependencyDefinition>() { definition };
            }
            else
            {
                definitions = definitions ?? new List<IDependencyDefinition>();    
            }
            
            return definitions.FirstOrDefault( x => x.PluginName == key );
        }

        public object GetInstance( Type serviceType )
        {
            return GetInstance( serviceType, null );
        }

        public object GetInstance( Type serviceType, string key )
        {
            var definition = GetDefinitionFor( serviceType, key );
            return GetInstanceFromDefinition( serviceType, definition );
        }

        public IEnumerable<object> GetAllInstances( Type serviceType )
        {
            var definitions = new List<IDependencyDefinition>();
            Definitions.TryGetValue( serviceType, out definitions );
            return definitions.Select( x => GetInstanceFromDefinition( serviceType, x ) );
        }

        public object GetInstanceFromDefinition( Type serviceType, IDependencyDefinition definition )
        {
            var provider = Providers.GetProviderForPlugin( serviceType, definition, this );
            if( provider == null )
                throw new ArgumentException( "No dependency has been configured for the request type: {0}",
                                             serviceType.ToString() );
            return provider.Get();
        }

        public T GetInstance<T>()
        {
            return (T) GetInstance( typeof( T ) );
        }

        public T GetInstance<T>( string key )
        {
            return (T) GetInstance( typeof( T ), key );
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            return GetAllInstances( typeof( T ) ).Cast<T>();
        }

        public SimpleDependencyRegistry()
        {
            Providers = new FactoryProvider();
            Definitions = new ConcurrentDictionary<Type, List<IDependencyDefinition>>();
        }
    }
}