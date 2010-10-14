using Machine.Specifications;

namespace Messaging.Tests.Local
{
    public class when_saving_actor_to_simple_cache
        : with_simple_actor_cache
    {
        protected static Actor actor { get; set; }

        private Because of = () =>
                                 {
                                     actor = new Actor() { Id = "Extra"};
                                     cache.Store(actor);
                                 };

        //private It should_retrieve_correct_instance = () => cache.GetOrAdd<Actor, string>("Extra", x => null).ShouldEqual(actor);
    }
}