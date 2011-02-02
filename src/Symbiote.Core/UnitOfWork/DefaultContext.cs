// /* 
// Copyright 2008-2011 Alex Robson
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
using System.Linq;

namespace Symbiote.Core.UnitOfWork
{
    public class DefaultContext<TActor> : IContext<TActor>
        where TActor : class
    {
        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }
        public IKeyAccessor<TActor> KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }
        public IList<IEvent> Events { get; set; }
        public Action<TActor> CommitAction { get; set; }
        public Action<TActor> SuccessAction { get; set; }
        public Action<TActor, Exception> ExceptionAction { get; set; }
        public IList<IDisposable> Disposables { get; set; }

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
            finally
            {
                Disposables.ToList().ForEach(a => a.Dispose());
            }
        }

        public void Commit()
        {
            if(CommitAction != null)
            {
                CommitAction( Actor );
            }
            Publisher.PublishEvents( Events );
        }

        public void PublishOnCommit<TEvent>( Action<TEvent> populateEvent )
        {
            var newEvent = Assimilate.GetInstanceOf<TEvent>();
            var baseEvent = newEvent as IEvent;
            PopulateDefaultEventFields( baseEvent );
            populateEvent( newEvent );
            Events.Add( baseEvent );
        }

        public void Rollback()
        {
            OriginalState.Reset( Actor );
        }

        protected void PopulateDefaultEventFields( IEvent baseEvent )
        {
            baseEvent.ActorId = KeyAccessor.GetId( Actor );
            baseEvent.ActorType = Actor.GetType().FullName;
            baseEvent.UtcTimeStamp = DateTime.UtcNow;
        }

        public DefaultContext(TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor, IEventPublisher eventPublisher)
        {
            Actor = actor;
            OriginalState = originalState;
            KeyAccessor = keyAccessor;
            Publisher = eventPublisher;
            Events = new List<IEvent>();
            Disposables = new List<IDisposable>();
        }
    }
}