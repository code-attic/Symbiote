using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Actor;

namespace Core.Tests.Actor.Agent
{
    public class when_requesting_agent_with_preloading
        : with_assimilation
    {
        public static Stopwatch firstRequestWatch { get; set; }
        public static IAgent<DummyActor> Agent { get; set; }

        private Because of = () =>
        {
            var agency = Assimilate.GetInstanceOf<IAgency>();

            firstRequestWatch = Stopwatch.StartNew();
            Agent = agency.GetAgentFor<DummyActor>();
            firstRequestWatch.Stop();
        };

        private It should_get_agent_instance = () => Agent.ShouldNotBeNull();
        private It should_take_under_20_ms = () => firstRequestWatch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 20 );
    }
}
