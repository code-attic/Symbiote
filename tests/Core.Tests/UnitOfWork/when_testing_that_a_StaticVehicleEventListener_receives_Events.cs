using System;
using System.Collections.Generic;
using System.Linq;
using Core.Tests.Actor.Domain.Model;
using Core.Tests.Domain.Listeners;
using Machine.Specifications;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.UnitOfWork
{
    public class when_testing_that_a_StaticVehicleEventListener_receives_Events : with_EventListenerManager
    {
        public static Vehicle Vehicle;
        public static Guid EventId = Guid.NewGuid();

        private Establish context = () =>
                                        {
                                            Vehicle = new Vehicle( "123456789", "Toyota", "4Runner", 1996 );
                                        };

        private Because of = () =>
                                 {
                                     using (var context = Context.CreateFor(Vehicle))
                                     {
                                         context.PublishOnCommit<VehicleGenericEvent>(x =>
                                                                                          {
                                                                                              x.EventId = EventId;
                                                                                          });
                                     }
                                 };

        private It should_have_heard_one_event = () =>
                                                     {
                                                         var dlist = new List<IEventListener>();
                                                         Manager.Listeners.TryGetValue(typeof(VehicleGenericEvent), out dlist);
                                                         dlist.OfType<StaticVehicleEventListener>().First().Events.Count.ShouldEqual( 1 );
                                                     };
        private It should_have_heard_the_event_with_the_correct_eventid = () =>
                                                                              {
                                                                                  var dlist = new List<IEventListener>();
                                                                                  Manager.Listeners.TryGetValue(typeof(VehicleGenericEvent), out dlist);
                                                                                  ShouldExtensionMethods.ShouldEqual( dlist.OfType<StaticVehicleEventListener>().First().Events[0].EventId, EventId);
                                                                              };
    }
}