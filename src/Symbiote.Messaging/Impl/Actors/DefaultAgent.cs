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

using System.Collections.Concurrent;
using System.Threading;
using Symbiote.Core.Extensions;

namespace Symbiote.Messaging.Impl.Actors
{
    public class DefaultAgent<TActor>
        : IAgent<TActor>
        where TActor : class
    {
        protected IActorCache<TActor> Cache { get; set; }
        protected IActorStore<TActor> Store { get; set; }
        protected IActorFactory<TActor> Factory { get; set; }
        protected ConcurrentDictionary<string, TActor> Actors { get; set; }
        protected ReaderWriterLockSlim SlimLock { get; set; }

        public TActor GetActor<TKey>(TKey key)
        {
            var stringKey = key.ToString();
            var actor = Actors.GetOrDefault(stringKey) 
                        ?? Cache.Get(key)
                        ?? Store.GetOrCreate(key);
            if (actor == null)
            {
                //SlimLock.EnterUpgradeableReadLock();
                if (!Actors.TryGetValue(stringKey, out actor))
                {
                    //SlimLock.EnterWriteLock();
                    "Creating actor for key {0}"
                        .ToInfo<IBus>(key);
                    actor = Factory.CreateInstance(key);
                    Actors.TryAdd(stringKey, actor);
                    //SlimLock.ExitWriteLock();
                }
                //SlimLock.ExitUpgradeableReadLock();
            }
            return actor;
        }

        public void Memoize(TActor actor)
        {
            Cache.Store(actor);
            //Store.Store(actor);
        }

        public DefaultAgent(IActorCache<TActor> cache, IActorStore<TActor> store, IActorFactory<TActor> factory)
        {
            Cache = cache;
            Store = store;
            Factory = factory;
            Actors = new ConcurrentDictionary<string, TActor>();
            SlimLock = new ReaderWriterLockSlim();
        }
    }
}