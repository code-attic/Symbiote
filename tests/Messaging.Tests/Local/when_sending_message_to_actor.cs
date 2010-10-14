using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Log.Impl;
using Symbiote.Messaging.Impl.Actors;
using Microsoft.Practices.ServiceLocation;

namespace Messaging.Tests.Local
{
    public class when_sending_message_to_actor
        : with_bus
    {
        protected static Actor actor { get; set; }
        protected static ILogger logger { get; set; }

        private Because of = () =>
                                 {
                                     bus.Send("", new KickRobotAss() { CorrelationId="Sam Worthington", Target = "Terminator"});
                                     actor = ServiceLocator.Current.GetInstance<IAgency>().GetAgentFor<Actor>().GetActor("Sam Worthington");
                                 };

        private It should_have_routed_message_to_actor_instance = () => 
                                                                  ShouldExtensionMethods.ShouldEqual(actor.FacesIveDrivenOver.First(), "Terminator");
    }
}