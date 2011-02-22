using Machine.Specifications;

namespace Core.Tests.Actor.Cache
{
    public class when_caching_actor
        : with_cache
    {
        public static CacheItem instance { get; set; }
        public static CacheItem retrieved { get; set; }

        private Because of = () =>
        {
            instance = new CacheItem() { Id = 1 };
            Cache.Store( Memoizer.GetMemento(instance) );
            retrieved = Memoizer.GetFromMemento(Cache.Get( 1 ));
        };

        private It should_store_and_retrieve_by_key = () => 
            instance.Id.ShouldEqual( retrieved.Id );
    }
}
