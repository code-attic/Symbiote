using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Actor.Impl;
using Symbiote.Core;

namespace Actor.Tests.Agent
{
    public class when_requesting_agent_with_preloading
        : with_assimilation
    {
        public static Stopwatch firstRequestWatch { get; set; }
        public static Stopwatch secondRequestWatch { get; set; }

        private Because of = () =>
        {
            var agency = Assimilate.GetInstanceOf<IAgency>();

            firstRequestWatch = Stopwatch.StartNew();
            var agent = agency.GetAgentFor<DummyActor>();
            firstRequestWatch.Stop();
        };
        
        private It should_not_take_more_than_1_milisecond = () => firstRequestWatch.ElapsedMilliseconds.ShouldBeLessThan( 1.1 );
    }
}
