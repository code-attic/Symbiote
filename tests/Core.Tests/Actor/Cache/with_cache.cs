using System;
using System.Collections.Generic;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Actor;
using Symbiote.Core.Memento;
using Symbiote.Core.UnitOfWork;

namespace Actor.Tests.Cache
{
    public class with_cache
        : with_assimilation
    {
        public static IActorCache<CacheItem> Cache { get; set; }
        public static IMemoizer Memoizer { get; set; }

        private Establish context = () =>
        {
            Cache = Assimilate.GetInstanceOf<MemoryCache<CacheItem>>();
            Memoizer = new Memoizer();
        };
    }

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

    public class MemoryStore<T> :
        IActorStore<T>
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

        public MemoryStore(IKeyAccessor<T> keyAccessor)
        {
            KeyAccessor = keyAccessor;
            store = new Dictionary<string, IMemento<T>>();
        }
    }
}