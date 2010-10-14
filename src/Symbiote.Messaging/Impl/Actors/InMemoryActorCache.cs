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
using System.Collections.Concurrent;

namespace Symbiote.Messaging.Impl.Actors
{
    public class InMemoryActorCache
        : IActorCache
    {
        protected ConcurrentDictionary<string, object> Actors { get; set; }
        protected IKeyAccessor KeyAccessor { get; set; }

        public TActor GetOrAdd<TActor, TKey>(TKey id, Func<TKey, TActor> createWith)
            where TActor : class
        {
            object actor = null;
            if(!Actors.TryGetValue(id.ToString(), out actor))
            {
                actor = createWith(id);
                if(!Actors.TryAdd(id.ToString(), actor))
                {
                    actor = Actors.GetOrAdd(id.ToString(), actor);
                }
            }
            return actor as TActor;
        }

        public void Store<TActor>(TActor actor)
            where TActor : class
        {
            var key = KeyAccessor.GetIdAsString(actor);
            Actors.TryAdd(key, actor);
        }

        public InMemoryActorCache(IKeyAccessor keyAccessor)
        {
            KeyAccessor = keyAccessor;
            Actors = new ConcurrentDictionary<string, object>();
        }
    }
}