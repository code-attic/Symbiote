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
using Symbiote.Core.Reflection;
using Symbiote.Core.DI;

namespace Symbiote.Core.Actor
{
    public class KeyAccessManager : IKeyAccessor
    {
        public ConcurrentDictionary<Type, IKeyAccessor> Adapters { get; set; }
        public List<Type> AccessorAvailabilityList { get; set; }

        public bool HasAccessFor( Type type )
        {
            return Adapters.ContainsKey( type );
        }

        public string GetId( object actor, Type type )
        {
            var accessor = GetAdapterFor( actor );
            return accessor.GetId( actor, type );
        }

        public string GetId<TActor>( TActor actor ) where TActor : class
        {
            var accessor = GetAdapterFor( actor );
            return accessor.GetId( actor );
        }

        public IKeyAccessor GetAdapterFor(object actor)
        {
            var type = actor.GetType();
            IKeyAccessor accessor;
            if( !Adapters.TryGetValue( type, out accessor ) )
            {
                var typeStack = Reflector.GetInheritanceChainFor(type);
                var accessorType = AccessorAvailabilityList.FirstOrDefault( typeStack.Contains );
                if(accessorType != null)
                {
                    var makeGenericType = typeof( KeyAccessAdapter<> ).MakeGenericType( accessorType );
                    accessor = Assimilate.GetInstanceOf( makeGenericType ) as IKeyAccessor;
                    Adapters.TryAdd( type, accessor );
                }
            }
            return accessor;
        }
        
        public void SetId( object actor, object key, Type type )
        {
            var accessor = GetAdapterFor( actor );
            accessor.SetId( actor, key, type );
        }

        public void SetId<TActor, TKey>( TActor actor, TKey key ) where TActor : class
        {
            var accessor = GetAdapterFor( actor );
            accessor.SetId( actor, key );
        }

        public KeyAccessManager()
        {
            Adapters = new ConcurrentDictionary<Type, IKeyAccessor>();
            var types = Assimilate
                .Assimilation
                .DependencyAdapter
                .RegisteredPluginTypes
                .Where( x => x.ImplementsInterfaceTemplate( typeof(IKeyAccessor<>) ) )
                .Select( x => x.GetInterface( "IKeyAccessor`1" )  );

            AccessorAvailabilityList = 
                types
                    .Select( k => k.GetGenericArguments().First() )
                    .ToList();
        }
    }
}
