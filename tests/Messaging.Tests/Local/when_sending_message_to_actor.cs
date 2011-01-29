using System;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Actor;
using Symbiote.Core.Log.Impl;
using Symbiote.Messaging;

namespace Messaging.Tests.Local
{
    public class when_sending_message_to_actor
        : with_bus
    {
        protected static Actor actor { get; set; }
        protected static ILogger logger { get; set; }

        private Because of = () =>
                                 {
                                     bus.AddLocalChannel(x => x.CorrelateBy<KickRobotAss>(m => m.CorrelationId));
                                     bus.Publish("local", new KickRobotAss() { CorrelationId="Sam Worthington", Target = "Terminator", Created = DateTime.Now});
                                     Thread.Sleep( 9 );
                                     actor = Assimilate.GetInstanceOf<IAgency>().GetAgentFor<Actor>().GetActor("Sam Worthington");
                                 };

        private It should_have_instantiated_actor_once = () =>
                                                         Actor.Created.ShouldEqual( 1 );

        private It should_have_routed_message_to_actor_instance = () => 
                                                                  actor.FacesIveDrivenOver.First().ShouldEqual("Terminator");
    }
}