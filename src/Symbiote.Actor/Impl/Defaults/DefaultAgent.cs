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

using Symbiote.Core.Memento;
using Symbiote.Core.Utility;

namespace Symbiote.Actor.Impl.Defaults
{
    public class DefaultAgent<TActor>
        : IAgent<TActor>
        where TActor : class
    {
        protected IActorCache<TActor> Cache { get; set; }
        protected IActorStore<TActor> Store { get; set; }
        protected IActorFactory<TActor> Factory { get; set; }
        protected IMemoizer Memoizer { get; set; }
        public ExclusiveConcurrentDictionary<string, TActor> Actors { get; set; }

        public TActor GetActor<TKey>(TKey key)
        {
            return Actors.ReadOrWrite( key.ToString(), 
                    () =>
                        {
                            var memento = Cache.Get(key) ?? Store.Get(key);
                            return memento != null ? memento.Retrieve() : Factory.CreateInstance(key);
                        });
        }

        public void RegisterActor<TKey>( TKey key, TActor actor )
        {
            Actors.ReadOrWrite( key.ToString(), () => actor );
        }

        public void Memoize(TActor actor)
        {
            //var memento = Memoizer.GetMemento( actor );
            //Cache.Store(memento);
            //Store.Store(memento);
        }

        public DefaultAgent(
            IActorCache<TActor> cache, 
            IActorStore<TActor> store, 
            IActorFactory<TActor> factory,
            IMemoizer memoizer)
        {
            Memoizer = memoizer;
            Cache = cache;
            Store = store;
            Factory = factory;
            Actors = new ExclusiveConcurrentDictionary<string, TActor>();
        }
    }
}