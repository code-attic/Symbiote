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

namespace Symbiote.Messaging.Impl.Actors
{
    public class DefaultAgent<TActor>
        : IAgent<TActor>
        where TActor : class
    {
        protected IActorCache Cache { get; set; }
        protected IActorStore<TActor> Store { get; set; }
        protected IActorFactory<TActor> Factory { get; set; }

        public TActor GetActor<TKey>(TKey key)
        {
            return Cache.GetOrAdd(key, k => Store.GetOrCreate(k, Factory.CreateInstance));
        }

        public void Memoize(TActor actor)
        {
            Cache.Store(actor);
            Store.Store(actor);
        }

        public DefaultAgent(IActorCache cache, IActorStore<TActor> store, IActorFactory<TActor> factory)
        {
            Cache = cache;
            Store = store;
            Factory = factory;
        }
    }
}