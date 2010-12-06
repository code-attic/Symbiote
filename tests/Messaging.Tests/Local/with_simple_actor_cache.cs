using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Actor.Impl.Actor.Defaults;

namespace Messaging.Tests.Local
{
    public class with_simple_actor_cache
        : with_assimilation
    {
        protected static IActorCache<Actor> cache { get; set; }
        private Establish context = () =>
                                        {
                                            cache = new NullActorCache<Actor>();
                                        };
    }
}