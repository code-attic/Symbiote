using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Actor.Agent
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
