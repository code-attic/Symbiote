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

namespace Symbiote.Core.UnitOfWork
{

    public class ReplayContext<TActor>
        : IContext
        where TActor : class
    {
        private Action<TActor> _successAction;
        private Action<TActor, Exception> _failureAction;
        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }

        public void Commit()
        {
            // does nothing
        }

        public void PublishOnCommit<TEvent>( Action<TEvent> populateEvent )
        {
            // does nothing
        }

        public void Rollback()
        {
            OriginalState.Reset( Actor );
        }

        public ReplayContext(TActor actor, IMemento<TActor> originalState) : this(actor, originalState, null, null)
        {
            
        }

        public ReplayContext(TActor actor, IMemento<TActor> originalState, Action<TActor> successAction, Action<TActor, Exception> failureAction)
        {
            Actor = actor;
            OriginalState = originalState;
            _successAction = successAction;
            _failureAction = failureAction;
        }

        public void Dispose()
        {
            try
            {
                Commit();
                if (_successAction != null)
                    _successAction( Actor );
            }
            catch ( Exception exception )
            {
                if (_failureAction != null)
                    _failureAction( Actor, exception );
                else
                    throw;
            }
        }
    }
}
