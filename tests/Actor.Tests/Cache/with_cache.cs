using Machine.Specifications;
using Moq;
using Symbiote.Actor;
using Symbiote.Core;
using Symbiote.Core.Memento;

namespace Actor.Tests.Cache
{
    public class with_cache
        : with_assimilation
    {
        public static IActorCache<CacheItem> Cache { get; set; }
        public static IMemoizer Memoizer { get; set; }

        private Establish context = () =>
        {
            Cache = Assimilate.GetInstanceOf<IActorCache<CacheItem>>();
            Memoizer = new Memoizer();
        };
    }
}