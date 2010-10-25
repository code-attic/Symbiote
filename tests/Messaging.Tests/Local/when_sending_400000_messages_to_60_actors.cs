using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Log.Impl;
using Symbiote.Messaging;
using Symbiote.Messaging.Impl.Actors;
using Symbiote.Messaging.Impl.Dispatch;

namespace Messaging.Tests.Local
{
    public class when_sending_messages_to_actors
        : with_bus
    {
        protected static List<Actor> cast { get; set; }
        protected static Stopwatch watch { get; set; }
        protected static int MessagesToSend = 800000;
        protected static int actorCount = 80;
        protected static IDispatcher dispatcher;
        private Because of = () =>
                                 {
                                     Actor.Created = 0;

                                     var names = Enumerable.Range(0, actorCount).Select(x => "Extra " + x).ToList();
                                     var message = Enumerable.Range(0, actorCount)
                                         .Select(x => new KickRobotAss() 
                                                          {CorrelationId = names[x], Target = "Terminator"})
                                         .ToList();

                                     bus.AddLocalChannelForMessageOf<KickRobotAss>();
                                     watch = Stopwatch.StartNew();
                                     for (int i = 0; i < MessagesToSend; i++)
                                     {
                                         bus.Send(message[i % actorCount]);
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
                                 };
        
        private It should_complete_in_1_second = () =>
                                                 watch.ElapsedMilliseconds.ShouldBeLessThan(2001);

        private It should_create_the_correct_number_of_actors = () =>
                                                                  Actor.Created.ShouldEqual(actorCount);

        private It should_have_sent_all_messages_through_dispatch = () =>
                                                            dispatcher.Count.ShouldEqual(MessagesToSend);
        
        private It should_have_sent_all_messages_to_actors = () =>
                                                            cast.Sum(x => x.FacesIveDrivenOver.Count).ShouldEqual(MessagesToSend);
    }
}