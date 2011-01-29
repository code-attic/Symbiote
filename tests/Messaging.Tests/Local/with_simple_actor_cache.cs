using Machine.Specifications;
using Symbiote.Core.Actor;

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