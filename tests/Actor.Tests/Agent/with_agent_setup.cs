using Machine.Specifications;
using Moq;
using Symbiote.Actor;
using Symbiote.Actor.Impl.Actor.Defaults;

namespace Actor.Tests.Agent
{
    public class with_agent_setup
        : with_assimilation
    {
        public static IActorCache<DummyActor> ActorCache { get; set; }
        public static Mock<IActorCache<DummyActor>> MockActorCache { get; set; }

        public static IActorStore<DummyActor> ActorStore { get; set; }
        public static Mock<IActorStore<DummyActor>> MockActorStore { get; set; }

        public static DefaultAgent<DummyActor> Agent { get; set; }
        public static DummyActorFactory ActorFactory = new DummyActorFactory();

        private Establish context = () =>
        {
            MockActorCache = new Mock<IActorCache<DummyActor>>();

            ActorCache = MockActorCache.Object;

            MockActorStore = new Mock<IActorStore<DummyActor>>();

            ActorStore = MockActorStore.Object;
            Agent = new DefaultAgent<DummyActor>( ActorCache, ActorStore, ActorFactory  );
        };
    }
}