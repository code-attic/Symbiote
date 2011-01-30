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
    public class DefaultContext
    {
        private static IContextProvider _provider;

        protected static IContextProvider ContextProvider
        {
            get
            {
                _provider = _provider ?? Assimilate.GetInstanceOf<IContextProvider>();
                return _provider;
            }
        }

        public static IContext CreateFor<TActr>( TActr instance )
            where TActr : class
        {
            return ContextProvider.GetContext( instance );
        }
    }

    public class DefaultContext<TActor> : IContext
        where TActor : class
    {
        private readonly Action<TActor, Exception> _failureAction;
        private readonly List<IDisposable> _subscriptionTokens = new List<IDisposable>();
        private readonly Action<TActor> _successAction;

        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }
        public IKeyAccessor<TActor> KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }
        public IList<IEvent> Events { get; set; }

        #region IContext Members

        public void Dispose()
        {
            try
            {
                Commit();
                _subscriptionTokens.ForEach( a => a.Dispose() );
                if ( _successAction != null )
                    _successAction( Actor );
            }
            catch ( Exception exception )
            {
                if ( _failureAction != null )
                    _failureAction( Actor, exception );
                else
                    throw;
            }
        }

        public void Commit()
        {
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

        #endregion

        protected void PopulateDefaultEventFields( IEvent baseEvent )
        {
            baseEvent.ActorId = KeyAccessor.GetId( Actor );
            baseEvent.ActorType = Actor.GetType().FullName;
            baseEvent.UtcTimeStamp = DateTime.UtcNow;
        }

        public DefaultContext( TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor,
                               IEventPublisher eventPublisher, IEnumerable<IObserver<IEvent>> listeners )
            : this( actor, originalState, keyAccessor, eventPublisher, listeners, null, null )
        {
        }

        public DefaultContext( TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor,
                               IEventPublisher eventPublisher )
            : this( actor, originalState, keyAccessor, eventPublisher, null, null, null )
        {
        }

        public DefaultContext( TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor,
                               IEventPublisher eventPublisher, Action<TActor> successAction,
                               Action<TActor, Exception> failureAction )
            : this( actor, originalState, keyAccessor, eventPublisher, null, successAction, failureAction )
        {
        }

        public DefaultContext( TActor actor, IMemento<TActor> originalState, IKeyAccessor<TActor> keyAccessor,
                               IEventPublisher eventPublisher, IEnumerable<IObserver<IEvent>> listeners,
                               Action<TActor> successAction, Action<TActor, Exception> failureAction )
        {
            Actor = actor;
            OriginalState = originalState;
            Events = new List<IEvent>();
            KeyAccessor = keyAccessor;
            Publisher = eventPublisher;
            if ( listeners != null )
                listeners.ToList().ForEach( a => _subscriptionTokens.Add( Publisher.Subscribe( a ) ) );
            _successAction = successAction;
            _failureAction = failureAction;
        }
    }
}