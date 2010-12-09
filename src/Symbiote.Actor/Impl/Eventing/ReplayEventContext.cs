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

namespace Symbiote.Actor.Impl.Eventing
{

    public class ReplayEventContext<TActor>
        : IEventContext
        where TActor : class
    {
        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }

        public void Commit()
        {
            // does nothing
        }

        public void Publish<TEvent>( Action<TEvent> populateEvent )
        {
            // does nothing
        }

        public void Rollback()
        {
            OriginalState.Reset( Actor );
        }

        public ReplayEventContext( TActor actor, IMemento<TActor> originalState )
        {
            Actor = actor;
            OriginalState = originalState;
        }

        public void Dispose()
        {
            Commit();
        }
    }
}
