using Machine.Specifications;
using Moq;
using Symbiote.Core.Actor;

namespace Actor.Tests.Agency
{
    public class with_mock_agent_factory :
        with_assimilation
    {
        public static Mock<IAgentFactory> MockAgentFactory { get; set; }
        public static IAgentFactory AgentFactory { get; set; }
        public static Symbiote.Core.Actor.Agency TestAgency { get; set; }

        private Establish context = () =>
        {
            MockAgentFactory = new Mock<IAgentFactory>();
            MockAgentFactory.Setup( x => x.GetAgentFor<DummyActor>() ).Returns( () => new DummyAgent() );

            AgentFactory = MockAgentFactory.Object;

            TestAgency = new Symbiote.Core.Actor.Agency( AgentFactory );
        };
    }
}