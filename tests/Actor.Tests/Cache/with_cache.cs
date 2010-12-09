using Machine.Specifications;
using Moq;
using Symbiote.Actor;
using Symbiote.Actor.Impl.Memento;
using Symbiote.Core;

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