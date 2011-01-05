using System;
using System.Collections.Concurrent;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Impl.Utility
{
    public class ObserverCollection<TEvent> 
    {
        protected ConcurrentDictionary<Guid, IObserver<TEvent>> Observers { get; set; }

        public IDisposable AddObserver(IObserver<TEvent> observer)
        {
            var token = new ObserverToken( RemoveObserver );
            Observers.TryAdd( token.Id, observer );
            return token;
        }

        public void OnEvent(TEvent e)
        {
            Notify(x => x.OnNext(e));
        }

        public void OnError(Exception e)
        {
            Notify(x => x.OnError(e));
        }

        public void OnComplete()
        {
            Notify( x => x.OnCompleted() );
        }

        public void Notify(Action<IObserver<TEvent>> action)
        {
            Observers.Values.ForEach(action);
        }

        public void RemoveObserver(Guid id)
        {
            IObserver<TEvent> observer;
            Observers.TryRemove( id, out observer );
        }

        public ObserverCollection()
        {
            Observers = new ConcurrentDictionary<Guid, IObserver<TEvent>>();
        }
    }
}