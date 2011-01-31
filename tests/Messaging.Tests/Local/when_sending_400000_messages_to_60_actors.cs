using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Actor;
using Symbiote.Core.Log.Impl;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Dispatch;

namespace Messaging.Tests.Local
{
    public class when_sending_messages_to_actors
        : with_bus
    {
        protected static List<Actor> cast { get; set; }
        protected static Stopwatch watch { get; set; }
        protected static int MessagesToSend = 400000;
        protected static int actorCount = 60;
        protected static IDispatcher dispatcher;
        private Because of = () =>
                                 {
                                     Actor.Created = 0;

                                     bus.AddLocalChannel(x => x.CorrelateBy<KickRobotAss>(m => m.CorrelationId));

                                     var names = Enumerable.Range(0, actorCount).Select(x => "Extra " + x).ToList();
                                     var message = Enumerable.Range(0, actorCount)
                                         .Select(x => new KickRobotAss() 
                                                          {CorrelationId = names[x], Target = "Terminator"})
                                         .ToList();

                                     watch = Stopwatch.StartNew();
                                     for (int i = 0; i < MessagesToSend; i++)
                                     {
                                         bus.Publish("local", message[i % actorCount]);
                                     }
                                     watch.Stop();

                                     cast = new List<Actor>();
                                     dispatcher = Assimilate.GetInstanceOf<IDispatcher>();
                                     var agency = Assimilate.GetInstanceOf<IAgency>();
                                     var logProvider = Assimilate.GetInstanceOf<ILogProvider>();
                                     var agent = agency.GetAgentFor<Actor>();
                                     for (int i = 0; i < actorCount; i++)
                                     {
                                         var actor = agent.GetActor("Extra " + i);
                                         cast.Add(actor);
                                     }
                                     int count = cast.Count;
                                 };
        
        //Ring buffer speeds:
        // 355987 / 2442 ms
        // 341588 / 1784 ms
        // 361558 / 1836 ms
        // 359202 / 1749 ms


        //Mailbox Processor speeds:
        // 400000 / 2498 ms
        // 400000 / 2484 ms
        // 400000 / 2565 ms
        // 400000 / 2358 ms

        
        private It should_complete_in_1_second = () =>
                                                 watch.ElapsedMilliseconds.ShouldBeLessThan(10);

        private It should_create_the_correct_number_of_actors = () =>
                                                                  Actor.Created.ShouldEqual(actorCount);

        private It should_have_sent_all_messages_through_dispatch = () =>
                                                            dispatcher.Count.ShouldEqual(MessagesToSend);
        
        private It should_have_sent_all_messages_to_actors = () =>
                                                            cast.Sum(x => x.FacesIveDrivenOver.Count).ShouldEqual(MessagesToSend);
    }
}