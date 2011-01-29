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
using System.Collections.Generic;
using Symbiote.Core.Memento;

namespace Symbiote.Core.UnitOfWork
{
    public class DefaultContextProvider : IContextProvider
    {
        public IEventConfiguration Configuration { get; set; }
        public IMemoizer Memoizer { get; set; }
        public IKeyAccessor KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }

        public DefaultContextProvider( IEventConfiguration configuration, IMemoizer memoizer, IEventPublisher publisher )
        {
            Configuration = configuration;
            Memoizer = memoizer;
            Publisher = publisher;
        }
        
        public IContext GetContext<TActor>( TActor actor )
            where TActor : class
        {
            return GetContext(actor, null, null, null);
        }

        public IContext GetContext<TActor>( TActor actor, IEnumerable<IObserver<IEvent>> listeners)
            where TActor : class
        {
            return GetContext(actor, listeners, null, null);
        }


        public IContext GetContext<TActor>(TActor actor, Action<TActor> successAction, Action<TActor, Exception> failureAction) where TActor : class
        {
            return GetContext(actor, null, successAction, failureAction);
        }


        public IContext GetContext<TActor>(TActor actor, IEnumerable<IObserver<IEvent>> listeners, Action<TActor> successAction, Action<TActor, Exception> failureAction) where TActor : class
        {
            var originalState = Memoizer.GetMemento(actor);
            if (Configuration.Replay)
            {
                return new ReplayContext<TActor>(actor, originalState);
            }
            var keyAccessor = Assimilate.GetInstanceOf<IKeyAccessor<TActor>>();
            return new DefaultContext<TActor>(actor, originalState, keyAccessor, Publisher, listeners, successAction, failureAction);
        }
    }
}