using Machine.Specifications;
using Symbiote.Actor.Impl.Memento;

namespace Actor.Tests.Agent
{
    public class with_stored_actor
        : with_agent_setup
    {
        private Establish context = () =>
        {
            MockActorCache.Setup(x => x.Get<string>(Moq.It.IsAny<string>())).Returns(new PassthroughMemento<DummyActor>());
            MockActorStore.Setup(x => x.Get<string>(Moq.It.IsAny<string>())).Returns(new PassthroughMemento<DummyActor>());
        };
    }
}