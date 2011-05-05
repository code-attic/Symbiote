// /* 
// Copyright 2008-2011 Jim Cowart & Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Collections.Generic;

namespace Symbiote.Core.UnitOfWork
{
    public class ReplayContext<TActor>
        : IContext<TActor>
        where TActor : class
    {
        public TActor Actor { get; set; }
        public Action<TActor> CommitAction { get; set; }
        public Action<TActor> SuccessAction { get; set; }
        public Action<TActor, Exception> ExceptionAction { get; set; }
        public IMemento<TActor> OriginalState { get; set; }
        public IList<Action<IDisposable>> DisposeActions { get; set; }
        public IEventPublisher Publisher
        {
            get { throw new NotImplementedException();}
            set { throw new NotImplementedException();}
        }

        public IList<IDisposable> Disposables { get; set; }

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

        public void Dispose()
        {
            try
            {
                Commit();
                if ( SuccessAction != null )
                    SuccessAction( Actor );
            }
            catch ( Exception exception )
            {
                if ( ExceptionAction != null )
                    ExceptionAction( Actor, exception );
                else
                    throw;
            }
        }

        public ReplayContext( TActor actor, IMemento<TActor> originalState )
        {
            Actor = actor;
            OriginalState = originalState;
        }
    }
}