using Symbiote.Core;

namespace Core.Tests.Actor.Cache
{
    public class CacheMemento : IMemento<CacheItem>
    {
        public int Id { get; set; }

        public void Capture( CacheItem instance )
        {
            Id = instance.Id;
        }

        public void Reset( CacheItem instance )
        {
            instance.Id = Id;
        }

        public CacheItem Retrieve()
        {
            return new CacheItem() { Id = Id };
        }
    }
}