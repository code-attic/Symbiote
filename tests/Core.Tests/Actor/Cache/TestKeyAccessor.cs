using Symbiote.Core;
using Symbiote.Core.UnitOfWork;

namespace Actor.Tests.Cache
{
    public class TestKeyAccessor
        : IKeyAccessor<CacheItem>
    {
        public string GetId( CacheItem actor )
        {
            return actor.Id.ToString();
        }

        public void SetId<TKey>( CacheItem actor, TKey key )
        {
            actor.Id = int.Parse( key.ToString() );
        }
    }
}