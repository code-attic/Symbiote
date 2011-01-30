using Machine.Specifications;
using Symbiote.Core.Memento;

namespace Actor.Tests.Agent
{
    public class with_stored_actor
        : with_agent_setup
    {
        private Establish context = () =>
        {
            MockActorCache.Setup(x => x.Get<string>(Moq.It.IsAny<string>())).Returns( default(PassthroughMemento<DummyActor>) );
            MockActorStore.Setup(x => x.Get<string>(Moq.It.IsAny<string>())).Returns(new PassthroughMemento<DummyActor>() { Actor = new DummyActor() });
        };
    }
}