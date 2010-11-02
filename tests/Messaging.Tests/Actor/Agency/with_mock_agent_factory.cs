using Machine.Specifications;
using Moq;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Actor.Agency
{
    public class with_mock_agent_factory :
        with_assimilation
    {
        public static Mock<IAgentFactory> MockAgentFactory { get; set; }
        public static IAgentFactory AgentFactory { get; set; }
        public static Symbiote.Messaging.Impl.Actors.Agency TestAgency { get; set; }

        private Establish context = () =>
        {
            MockAgentFactory = new Mock<IAgentFactory>();
            MockAgentFactory.Setup( x => x.GetAgentFor<DummyActor>() ).Returns( () => new DummyAgent() );

            AgentFactory = MockAgentFactory.Object;

            TestAgency = new Symbiote.Messaging.Impl.Actors.Agency( AgentFactory );
        };
    }
}