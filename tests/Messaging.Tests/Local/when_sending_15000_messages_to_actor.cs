using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;

namespace Messaging.Tests.Local
{
    public class when_sending_15000_messages_to_actor
        : with_bus
    {
        protected static Actor actor { get; set; }
        protected static Stopwatch watch { get; set; }
        protected static int MessagesToSend = 15000;
        private Because of = () =>
                                 {
                                     Actor.Created = 0;

                                     bus.AddLocalChannelForMessageOf<KickRobotAss>( x => x.CorrelateByMessageProperty( m => m.CorrelationId ) );

                                     watch = Stopwatch.StartNew();
                                     for (int i = 0; i < MessagesToSend; i++)
                                     {
                                         bus.Publish(new KickRobotAss() { CorrelationId = "Sam Worthington", Target = "Terminator" });
                                     }
                                     watch.Stop();
                                     actor = Assimilate.GetInstanceOf<IAgency>().GetAgentFor<Actor>().GetActor("Sam Worthington");
                                 };

        private It should_complete_in_1_second = () =>
                                                 watch.ElapsedMilliseconds.ShouldBeLessThan(1001);

        private It should_only_have_created_the_actor_once = () =>
                                                             Actor.Created.ShouldEqual(1);

        private It should_have_sent_all_messages_to_actor = () =>
                                                            actor.FacesIveDrivenOver.Count.ShouldEqual(MessagesToSend);
    }
}