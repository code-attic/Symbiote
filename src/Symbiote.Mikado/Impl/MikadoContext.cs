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
using Symbiote.Core.Impl.UnitOfWork;

namespace Symbiote.Mikado.Impl
{
    public class MikadoContext<TActor> : IContext where TActor : class
    {
        private readonly List<IDisposable> _subscriptionTokens = new List<IDisposable>();
        private readonly IRunRules _rulesRunner;
        private Action<TActor> _successAction;
        private Action<TActor, Exception> _failureAction;

        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }
        public IKeyAccessor<TActor> KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }
        public IList<IEvent> Events { get; set; }
        public BrokenRulesCollection BrokenRules { get; set; }

        public MikadoContext(TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor, IEventPublisher eventPublisher, IRunRules rulesRunner)
            : this(actor, originalState, keyAccessor, eventPublisher, rulesRunner, null, null, null)
        {
            
        }

        public MikadoContext(TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor, IEventPublisher eventPublisher, IRunRules rulesRunner, IEnumerable<IObserver<IEvent>> listeners)
            : this(actor, originalState, keyAccessor, eventPublisher, rulesRunner, listeners, null, null)
        {

        }

        public MikadoContext( TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor, IEventPublisher eventPublisher, IRunRules rulesRunner, IEnumerable<IObserver<IEvent>> listeners, Action<TActor> successAction, Action<TActor,Exception> failureAction )
        {
            Actor = actor;
            OriginalState = originalState;
            KeyAccessor = keyAccessor;
            Publisher = eventPublisher;
            _rulesRunner = rulesRunner;
            BrokenRules = new BrokenRulesCollection();
            Events = new List<IEvent>();
            _rulesRunner.Subscribe(BrokenRules);
            if(listeners != null)
                listeners.ToList().ForEach(a => _subscriptionTokens.Add(Publisher.Subscribe(a)));
            _successAction = successAction;
            _failureAction = failureAction;
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
                if (_failureAction != null)
                    _failureAction(Actor, exception);
                else
                    throw;
            }
        }

        public void Commit()
        {
            _rulesRunner.ApplyRules(Actor);
            if (BrokenRules.Count == 0)
            {
                Publisher.PublishEvents(Events);
                if (_successAction != null)
                    _successAction(Actor);
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

        #region Static Members...
        private static IContextProvider _provider;

        protected static IContextProvider ContextProvider
        {
            get
            {
                _provider = _provider ?? Assimilate.GetInstanceOf<IContextProvider>();
                return _provider;
            }
        }

        public static IContext CreateFor<TActor>(TActor instance)
            where TActor : class
        {
            return ContextProvider.GetContext(instance);
        }
        #endregion
    }
}
