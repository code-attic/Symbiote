using System;
using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Core.Impl.Memento;
using Symbiote.Core.Impl.UnitOfWork;

namespace Symbiote.Mikado.Impl
{
    public class MikadoContextProvider : IContextProvider
    {
        private readonly IRunRules _rulesRunner;

        public IEventConfiguration Configuration { get; set; }
        public IMemoizer Memoizer { get; set; }
        public IEventPublisher Publisher { get; set; }

        public MikadoContextProvider( IEventConfiguration configuration, IMemoizer memoizer, IEventPublisher publisher, IRunRules rulesRunner )
        {
            Configuration = configuration;
            Memoizer = memoizer;
            Publisher = publisher;
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
            var keyAccessor = Assimilate.GetInstanceOf <IKeyAccessor<TActor>>();
            return new MikadoContext<TActor>(actor, originalState, keyAccessor, Publisher, _rulesRunner, listeners);
        }
    }
}