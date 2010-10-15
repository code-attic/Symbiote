using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;
using Symbiote.Messaging.Impl.Dispatch;

namespace Messaging.Tests.Local
{
    public class when_sending_400000_messages_to_60_actors
        : with_bus
    {
        protected static List<Actor> cast { get; set; }
        protected static Stopwatch watch { get; set; }
        protected static int MessagesToSend = 1000000;
        protected static int actorCount = 50;
        private Because of = () =>
                                 {
                                     Actor.Created = 0;

                                     var names = Enumerable.Range(0, actorCount).Select(x => "Extra " + x).ToList();
                                     var message = Enumerable.Range(0, actorCount)
                                         .Select(x => new KickRobotAss() 
                                                          {CorrelationId = names[x], Target = "Terminator"})
                                         .ToList();

                                     bus.AddLocalEndpoint("");
                                     watch = Stopwatch.StartNew();
                                     for (int i = 0; i < MessagesToSend; i++)
                                     {
                                         bus.Send("", message[i % actorCount]);
                                     }
                                     watch.Stop();

                                     cast = new List<Actor>();
                                     var manager = ServiceLocator.Current.GetInstance<IDispatcher>();
                                     var agency = ServiceLocator.Current.GetInstance<IAgency>();
                                     var agent = agency.GetAgentFor<Actor>();
                                     for (int i = 0; i < actorCount; i++)
                                     {
                                         var actor = agent.GetActor("Extra " + i);
                                         cast.Add(actor);
                                     }

                                     Thread.Sleep(TimeSpan.FromSeconds(2));
                                 };
        
        private It should_complete_in_1_second = () =>
                                                 watch.ElapsedMilliseconds.ShouldBeLessThan(1001);

        private It should_only_have_created_the_actor_60_times = () =>
                                                                  Actor.Created.ShouldEqual(actorCount);

        private It should_have_sent_all_messages_to_actor = () =>
                                                            cast.Sum(x => x.FacesIveDrivenOver.Count).ShouldEqual(MessagesToSend);
    }
}