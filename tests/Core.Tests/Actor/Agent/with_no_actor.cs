using Machine.Specifications;
using Symbiote.Core.Memento;

namespace Core.Tests.Actor.Agent
{
    public class with_no_actor
        : with_agent_setup
    {
        private Establish context = () =>
        {
            MockActorCache.Setup( x => x.Get<string>( Moq.It.IsAny<string>() ) ).Returns( default(PassthroughMemento<DummyActor>) );
            MockActorStore.Setup(x => x.Get<string>(Moq.It.IsAny<string>())).Returns(default(PassthroughMemento<DummyActor>));
        };
    }
}