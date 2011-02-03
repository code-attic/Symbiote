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
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;

namespace Symbiote.Mikado.Impl
{
    public class MikadoContext<TActor> : IContext<TActor> where TActor : class
    {
        private readonly List<IDisposable> _subscriptionTokens = new List<IDisposable>();
        private readonly IRunRules _rulesRunner;

        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }
        public IKeyAccessor KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }
        public IList<IEvent> Events { get; set; }
        public BrokenRulesCollection BrokenRules { get; set; }
        public Action<TActor> SuccessAction { get; set; }
        public Action<TActor> CommitAction { get; set; }
        public Action<TActor, Exception> ExceptionAction { get; set; }
        public IList<IDisposable> Disposables { get; set; }

        public MikadoContext(TActor actor, IMemento<TActor> originalState, IKeyAccessor keyAccessor, IEventPublisher eventPublisher, IRunRules rulesRunner )
        {
            BrokenRules = new BrokenRulesCollection();
            Events = new List<IEvent>(); 
            Disposables = new List<IDisposable>();
            Actor = actor;
            OriginalState = originalState;
            KeyAccessor = keyAccessor;
            Publisher = eventPublisher;
            _rulesRunner = rulesRunner;
            _rulesRunner.Subscribe(BrokenRules);
        }

        public void Dispose()
        {
            try
            {
                Commit();
                _subscriptionTokens.ForEach(a => a.Dispose());
            }
            catch (Exception exception)
            {
                if (ExceptionAction != null)
                    ExceptionAction(Actor, exception);
                else
                    throw;
            }
        }

        public void Commit()
        {
            if(CommitAction != null)
            {
                CommitAction( Actor );
            }
            _rulesRunner.ApplyRules( Actor );
            if (BrokenRules.Count == 0)
            {
                Publisher.PublishEvents( Events );
                if (SuccessAction != null)
                    SuccessAction( Actor );
            }
            else
            {
                if (OnBrokenRules != null)
                    OnBrokenRules( Actor, BrokenRules );
                Rollback();
            }
        }

        public void PublishOnCommit<TEvent>( Action<TEvent> populateEvent )
        {
            var newEvent = Assimilate.GetInstanceOf<TEvent>();
            var baseEvent = newEvent as IEvent;
            PopulateDefaultEventFields (baseEvent);
            populateEvent( newEvent );
            Events.Add( baseEvent );
        }

        protected void PopulateDefaultEventFields( IEvent baseEvent ) 
        {
            baseEvent.ActorId = KeyAccessor.GetId( Actor );
            baseEvent.ActorType = Actor.GetType().FullName;
            baseEvent.UtcTimeStamp = DateTime.UtcNow;
        }

        public void Rollback()
        {
            OriginalState.Reset( Actor );
        }

        public Action<TActor, IList<IBrokenRuleNotification>> OnBrokenRules { get; set; }
    }
}
