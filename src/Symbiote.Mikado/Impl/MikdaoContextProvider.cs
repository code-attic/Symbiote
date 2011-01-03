using System;
using System.Collections.Generic;
using Symbiote.Core.Memento;
using Symbiote.Core.Work;

namespace Symbiote.Mikado.Impl
{
    public class MikdaoContextProvider : IContextProvider
    {
        private readonly IRunRules _rulesRunner;

        public IEventConfiguration Configuration { get; set; }
        public IMemoizer Memoizer { get; set; }
        public IKeyAccessor KeyAccessor { get; set; }
        public IEventPublisher Publisher { get; set; }

        public MikdaoContextProvider( IEventConfiguration configuration, IMemoizer memoizer, IEventPublisher publisher, IKeyAccessor keyAccessor, IRunRules rulesRunner )
        {
            Configuration = configuration;
            Memoizer = memoizer;
            Publisher = publisher;
            KeyAccessor = keyAccessor;
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

            return new MikadoContext<TActor>(actor, originalState, KeyAccessor, Publisher, _rulesRunner, listeners);
        }
    }
}