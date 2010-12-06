using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Actor.Tests.Agent
{
    public class when_requesting_same_actor_under_race_condition
        : with_no_actor
    {
        protected static DummyActor Actor { get; set; }
        private Because of = () =>
        {
            Enumerable
                .Range( 0, 4 )
                .AsParallel()
                .ForAll( x => Actor = Agent.GetActor( "test" ) );
        };

        private It should_call_the_cache_once = () => MockActorCache.Verify( x => x.Get( "test" ), Times.Once());
        private It should_call_the_store_once = () => MockActorStore.Verify(x => x.Get("test"), Times.Once());
        private It should_call_the_factory_once = () => ActorFactory.Called.ShouldEqual( 1 );
        private It should_produce_actor_instance = () => Actor.ShouldNotBeNull();
        private It should_only_instantiate_one_actor = () => DummyActor.Instantiated.ShouldEqual( 1 );
    }
}
