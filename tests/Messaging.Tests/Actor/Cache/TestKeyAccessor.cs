using Symbiote.Messaging;

namespace Messaging.Tests.Actor.Cache
{
    public class TestKeyAccessor
        : IKeyAccessor<CacheItem>
    {
        public string GetId( CacheItem actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( CacheItem actor, TKey id )
        {
            actor.Id = int.Parse( id.ToString() );
        }
    }
}