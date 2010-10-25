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
using System.Threading;

namespace Symbiote.Messaging.Impl.Actors
{
    public class InMemoryActorCache<TActor>
        : IActorCache<TActor>
        where TActor : class
    {
        protected ReaderWriterLockSlim SlimLock { get; set; }
        protected ConcurrentDictionary<string, object> Actors { get; set; }
        protected IKeyAccessor<TActor> KeyAccessor { get; set; }
        
        public TActor Get<TKey>(TKey id)
        {
            object actor = null;
            Actors.TryGetValue(id.ToString(), out actor);
            return actor as TActor;
        }

        public void Store(TActor actor)
        {
            if(actor != null)
            {
                var key = KeyAccessor.GetId(actor);
                Actors.AddOrUpdate(key, actor, (x, y) => actor);
            }
        }

        public InMemoryActorCache(IKeyAccessor<TActor> keyAccessor)
        {
            KeyAccessor = keyAccessor;
            Actors = new ConcurrentDictionary<string, object>();
            SlimLock = new ReaderWriterLockSlim();
        }
    }
}