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
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Actor
{
    public class KeyAccessAdapter<T>
        : IKeyAccessor
        where T : class
    {
        public IKeyAccessor<T> Accessor { get; set; }

        public string GetId( object actor, Type type )
        {
            var actorType = actor.GetType();
            try
            {
                return Accessor.GetId( actor as T );
            }
            catch ( Exception )
            {
                throw new InvalidCastException(
                    "Key accessor cannot access an actor of {0} as type {1}".AsFormat( actorType, type ) );
            }
        }

        public string GetId<TActor>( TActor actor ) where TActor : class
        {
            if ( typeof( T ).IsAssignableFrom( typeof( TActor ) ) )
            {
                return Accessor.GetId( actor as T );
            }
            throw new InvalidCastException(
                "Key accessor cannot access an actor of {0} as type {1}".AsFormat( typeof( TActor ), typeof( T ) ) );
        }

        public void SetId( object actor, object key, Type type )
        {
            var actorType = actor.GetType();
            try
            {
                Accessor.SetId( actor as T, key );
            }
            catch ( Exception )
            {
                throw new InvalidCastException(
                    "Key accessor cannot access an actor of {0} as type {1}".AsFormat( actorType, type ) );
            }
        }

        public void SetId<TActor, TKey>( TActor actor, TKey key ) where TActor : class
        {
            if ( typeof( T ).IsAssignableFrom( typeof( TActor ) ) )
            {
                Accessor.SetId( actor as T, key );
            }
            else
            {
                throw new InvalidCastException(
                    "Key accessor cannot access an actor of {0} as type {1}".AsFormat( typeof( TActor ), typeof( T ) ) );
            }
        }

        public KeyAccessAdapter( IKeyAccessor<T> accessor )
        {
            Accessor = accessor;
        }
    }
}