using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Core.Tests.Actor.Agent
{
    public class when_requesting_stored_actor
        : with_stored_actor
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
        private It should_not_call_the_store = () => MockActorStore.Verify(x => x.Get("test"), Times.Once());
        private It should_not_call_the_factory = () => ActorFactory.Called.ShouldEqual(0);
        private It should_produce_actor_instance = () => Actor.ShouldNotBeNull();
        private It should_not_instantiate_actor = () => DummyActor.Instantiated.ShouldEqual(0);
    }
}