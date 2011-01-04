using Symbiote.Actor;
using Symbiote.Core.Work;

namespace Actor.Tests.Cache
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