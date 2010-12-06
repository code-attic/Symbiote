using System.Diagnostics;
using Machine.Specifications;
using Symbiote.Actor;
using Symbiote.Core;
using Symbiote.Messaging;

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

                                     bus.AddLocalChannel( x => x.CorrelateBy<KickRobotAss>( m => m.CorrelationId ) );

                                     watch = Stopwatch.StartNew();
                                     for (int i = 0; i < MessagesToSend; i++)
                                     {
                                         bus.Publish(new KickRobotAss() { CorrelationId = "Sam Worthington", Target = "Terminator" });
                                     }

                                     actor = Assimilate.GetInstanceOf<IAgency>().GetAgentFor<Actor>().GetActor("Sam Worthington");
                                     while(actor.FacesIveDrivenOver.Count < .99 * MessagesToSend)
                                     {
                                         
                                     }
                                     watch.Stop();
                                 };

        // performance is currently close to a factor of 100. Messages dispatched = 100 * ms run to send
        // new record: 83ms = 15000 > 180 msgs/ms
        // old record: 115ms = 15000 > 100 msg/ms
        // slowest: 198ms = 15000 ~ 75 msg/ms
        
        private It should_complete_in_1_second = () =>
                                                 watch.ElapsedMilliseconds.ShouldBeLessThan(11);

        private It should_only_have_created_the_actor_once = () =>
                                                             Actor.Created.ShouldEqual(1);

        private It should_have_sent_all_messages_to_actor = () =>
                                                            actor.FacesIveDrivenOver.Count.ShouldEqual(MessagesToSend);
    }
}