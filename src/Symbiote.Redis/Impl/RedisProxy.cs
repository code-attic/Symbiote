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
using Symbiote.Core;
using Symbiote.Core.Persistence;

namespace Symbiote.Redis.Impl 
{
    /// <summary>
    /// This class provides generic abstractions that Symbiote's
    /// Actor system requires in order to handle caching and
    /// persistence concerns. Not recommended for use outside
    /// of this use case unless you truly understand the
    /// underlying calls this makes to the IRedisClient API
    /// </summary>
    public class RedisProxy :
        IKeyValueStore, 
        IRepository
    {
        public IKeyAccessor KeyAccessor { get; set; }
        public IRedisClient Client { get; set; }

        public bool Delete<T>( string key )
        {
            return Client.Remove( key );
        }

        public T Get<T>( string key )
        {
            return Client.Get<T>( key );
        }

        public IEnumerable<T> GetAll<T>()
        {
            throw new NotImplementedException( "Redis does not currently support this operation as there are no buckets and no concept of record type." );
        }

        public bool Persist<T>( string key, T instance )
        {
            return Client.Set( key, instance );
        }

        public bool Delete<T>( T instance ) where T : class
        {
            return Client.Remove( KeyAccessor.GetId( instance ) );
        }

        public bool Persist<T>( T instance ) where T : class
        {
            return Client.Set( KeyAccessor.GetId( instance ), instance );
        }

        public RedisProxy( IKeyAccessor keyAccessor, IRedisClient client )
        {
            KeyAccessor = keyAccessor;
            Client = client;
        }
    }
}
