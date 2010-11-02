using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Messaging.Tests.Actor.Agency
{
    class when_requesting_agents_in_parallel
        : with_mock_agent_factory
    {
        private static int requestsToMake = 20;
        private Because of = () =>
        {
            Enumerable
                .Repeat( typeof(DummyActor), requestsToMake )
                .AsParallel()
                .ForAll( x => TestAgency.GetAgentFor<DummyActor>() );
        };

        private It should_create_one_agent = () => DummyAgent.Instantiated.ShouldEqual(1);
        private It should_request_agent_specified_times = () => MockAgentFactory.Verify(x => x.GetAgentFor<DummyActor>(), Times.AtMost( 1 ));
        private It should_have_several_in_waiting = () => TestAgency.Agents.MostWaiting.ShouldBeGreaterThan( 1 );
        private It should_retain_first_instance = () => (TestAgency.Agents[typeof(DummyActor)] as DummyAgent).InstanceId = 1;
    }
}
