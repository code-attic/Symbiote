using Machine.Specifications;
using Symbiote.Core.Memento;

namespace Core.Tests.Actor.Agent
{
    public class with_cached_actor 
        : with_agent_setup
    {
        private Establish context = () =>
        {
            MockActorCache.Setup(x => x.Get<string>(Moq.It.IsAny<string>())).Returns(new PassthroughMemento<DummyActor>() { Actor = new DummyActor() });
            MockActorStore.Setup(x => x.Get<string>(Moq.It.IsAny<string>())).Returns(new PassthroughMemento<DummyActor>());
        };
    }
}