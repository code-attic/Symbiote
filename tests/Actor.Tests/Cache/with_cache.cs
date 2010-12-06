using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Core;

namespace Actor.Tests.Cache
{
    public class with_cache
        : with_assimilation
    {
        public static IActorCache<CacheItem> Cache { get; set; }

        private Establish context = () =>
        {
            Cache = Assimilate.GetInstanceOf<IActorCache<CacheItem>>();
        };
    }
}