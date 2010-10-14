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

using Symbiote.Core;
using Microsoft.Practices.ServiceLocation;

namespace Symbiote.Messaging.Impl.Actors
{
    public class DefaultActorFactory<TActor>
        : IActorFactory<TActor>
        where TActor : class
    {
        protected IKeyAccessor KeyAccessor { get; set; }

        public TActor CreateInstance<TKey>(TKey id)
        {
            var actor = ServiceLocator.Current.GetInstance<TActor>();
            System.Linq.Observable.Generate(0, x => x < 10, x => x + 1, x => x);
            KeyAccessor.SetId(actor, id);
            return actor;
        }

        public DefaultActorFactory(IKeyAccessor keyAccessor)
        {
            KeyAccessor = keyAccessor;
        }
    }
}