using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Core.Actor;

namespace Core.Tests.Actor.Cache
{
    public class MemoryCache<T> :
        IActorCache<T>
        where T : class
    {
        public IKeyAccessor<T> KeyAccessor { get; set; }
        public Dictionary<string, IMemento<T>> store { get; set; }
        public IMemento<T> Get<TKey>( TKey id )
        {
            IMemento<T> memento;
            store.TryGetValue( id.ToString(), out memento );
            return memento;
        }

        public void Store( IMemento<T> actor )
        {
            store[KeyAccessor.GetId( actor.Retrieve() )] = actor;
        }

        public MemoryCache( IKeyAccessor<T> keyAccessor )
        {
            KeyAccessor = keyAccessor;
            store = new Dictionary<string, IMemento<T>>();
        }
    }
}