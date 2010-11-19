using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Actor.Cache
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