using Machine.Specifications;
using Symbiote.Messaging.Impl;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Local
{
    public class with_simple_actor_cache
        : with_assimilation
    {
        protected static IActorCache<Actor> cache { get; set; }
        private Establish context = () =>
                                        {
                                            cache = new InMemoryActorCache<Actor>(new ActorKeyAccessor());
                                        };
    }
}