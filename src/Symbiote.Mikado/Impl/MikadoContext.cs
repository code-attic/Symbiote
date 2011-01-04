using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Work;

namespace Symbiote.Mikado.Impl
{
    public class MikadoContext<TActor> : IContext where TActor : class
    {
        private List<IDisposable> _subscriptionTokens = new List<IDisposable>();
        private IRunRules _rulesRunner;

        public TActor Actor { get; set; }
        public IMemento<TActor> OriginalState { get; set; }
        public IKeyAccessor KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }
        public IList<IEvent> Events { get; set; }
        public BrokenRulesCollection BrokenRules { get; set; }

        public MikadoContext(
            TActor actor,
            IMemento<TActor> originalState,
            IKeyAccessor keyAccessor,
            IEventPublisher eventPublisher,
            IRunRules rulesRunner,
            IEnumerable<IObserver<IEvent>> listeners)
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
        }

        public MikadoContext(TActor actor, IMemento<TActor> originalState, IKeyAccessor keyAccessor, IEventPublisher eventPublisher, IRunRules rulesRunner)
            : this(actor, originalState, keyAccessor, eventPublisher, rulesRunner, null)
        {
            
        }

        public void Dispose()
        {
            Commit();
            _subscriptionTokens.ForEach(a => a.Dispose());
        }

        public void Commit()
        {
            _rulesRunner.ApplyRules(Actor);
            if (BrokenRules.Count == 0)
            {
                Publisher.PublishEvents(Events);
            }
            else
            {
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
