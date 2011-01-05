/* 
Copyright 2008-2010 Jim Cowart

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
using Symbiote.Core;
using Symbiote.Core.Impl.Memento;
using Symbiote.Core.Impl.UnitOfWork;

namespace Symbiote.Mikado.Impl
{
    public class MikadoContextProvider : IContextProvider
    {
        private readonly IRunRules _rulesRunner;

        public IEventConfiguration Configuration { get; set; }
        public IMemoizer Memoizer { get; set; }
        public IEventPublisher Publisher { get; set; }

        public MikadoContextProvider( IEventConfiguration configuration, IMemoizer memoizer, IEventPublisher publisher, IRunRules rulesRunner )
        {
            Configuration = configuration;
            Memoizer = memoizer;
            Publisher = publisher;
            _rulesRunner = rulesRunner;
        }

        public IContext GetContext<TActor>(TActor actor)
            where TActor : class
        {
            return GetContext(actor, null);
        }

        public IContext GetContext<TActor>(TActor actor, IEnumerable<IObserver<IEvent>> listeners)
            where TActor : class
        {
            var originalState = Memoizer.GetMemento(actor);
            if (Configuration.Replay)
            {
                return new ReplayContext<TActor>(actor, originalState);
            }
            var keyAccessor = Assimilate.GetInstanceOf <IKeyAccessor<TActor>>();
            return new MikadoContext<TActor>(actor, originalState, keyAccessor, Publisher, _rulesRunner, listeners);
        }
    }
}