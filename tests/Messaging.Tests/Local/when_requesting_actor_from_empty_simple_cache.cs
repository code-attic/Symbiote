using Machine.Specifications;

namespace Messaging.Tests.Local
{
    public class when_requesting_actor_from_empty_simple_cache
        : with_simple_actor_cache
    {
        protected static Actor actor { get; set; }

        private Because of = () =>
                                 {
                                     actor = new Actor();
                                 };

        //private It should_retrieve_default_instance = () => actor.Id.ShouldEqual(cache.GetOrAdd<Actor, string>("Extra", x => null).Id);
    }
}