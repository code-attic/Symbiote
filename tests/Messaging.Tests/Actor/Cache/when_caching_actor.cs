using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Actor.Cache
{
    public class TestCache
    {
        public int Id { get; set; }
    }

    public class TestKeyAccessor
        : IKeyAccessor<TestCache>
    {
        public string GetId( TestCache actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( TestCache actor, TKey id )
        {
            actor.Id = int.Parse( id.ToString() );
        }
    }

    public class with_cache
        : with_assimilation
    {
        public static IActorCache<TestCache> Cache { get; set; }

        private Establish context = () =>
        {
            Cache = Assimilate.GetInstanceOf<IActorCache<TestCache>>();
        };
    }

    public class when_caching_actor
        : with_cache
    {
        public static TestCache instance { get; set; }
        public static TestCache retrieved { get; set; }

        private Because of = () =>
        {
            instance = new TestCache() { Id = 1 };
            Cache.Store( instance );
            retrieved = Cache.Get( 1 );
        };

        private It should_store_and_retrieve_by_key = () => instance.Id.ShouldEqual( retrieved.Id );
    }
}
