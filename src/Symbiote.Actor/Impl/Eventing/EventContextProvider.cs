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
using Symbiote.Core.Memento;

namespace Symbiote.Actor.Impl.Eventing
{
    public class EventContextProvider
        : IEventContextProvider
    {
        public IEventConfiguration Configuration { get; set; }
        public IMemoizer Memoizer { get; set; }
        public IEventPublisher Publisher { get; set; }
        
        public IEventContext GetContext<TActor>( TActor actor )
            where TActor : class
        {
            var originalState = Memoizer.GetMemento( actor );
            if(Configuration.Replay)
            {
                return new ReplayEventContext<TActor>( actor, originalState );
            }
            var keyAccessor = Assimilate.GetInstanceOf<IKeyAccessor<TActor>>();
            return new EventContext<TActor>(actor, originalState, keyAccessor, Publisher);
        }

        public EventContextProvider( IEventConfiguration configuration, IMemoizer memoizer )
        {
            Configuration = configuration;
            Memoizer = memoizer;
        }
    }
}