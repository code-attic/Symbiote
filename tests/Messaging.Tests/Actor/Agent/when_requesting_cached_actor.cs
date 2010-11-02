using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Messaging.Tests.Actor.Agent
{
    public class when_requesting_cached_actor
        : with_cached_actor
    {
        protected static DummyActor Actor { get; set; }
        private Because of = () =>
        {
            //Reset Actor instance before calling
            DummyActor.Instantiated = 0;

            Enumerable
                .Range(0, 20)
                .AsParallel()
                .ForAll(x => Actor = Agent.GetActor("test"));
        };

        private It should_call_the_cache_once = () => MockActorCache.Verify(x => x.Get("test"), Times.Once());
        private It should_not_call_the_store = () => MockActorStore.Verify(x => x.Get("test"), Times.Never());
        private It should_not_call_the_factory = () => ActorFactory.Called.ShouldEqual(0);
        private It should_produce_actor_instance = () => Actor.ShouldNotBeNull();
        private It should_not_instantiate_actor = () => DummyActor.Instantiated.ShouldEqual(0);
        private It should_have_had_concurrent_requests = () => Agent.Actors.MostWaiting.ShouldBeGreaterThan(1);
    }
}