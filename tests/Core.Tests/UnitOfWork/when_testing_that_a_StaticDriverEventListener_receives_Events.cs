using System;
using System.Collections.Generic;
using System.Linq;
using Core.Tests.Actor.Domain.Model;
using Core.Tests.Domain.Listeners;
using Machine.Specifications;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.UnitOfWork
{
    public class when_testing_that_a_StaticDriverEventListener_receives_Events : with_EventListenerManager
    {
        public static Driver Driver;
        public static Guid EventId = Guid.NewGuid();

        private Establish context = () =>
                                        {
                                            Driver = new Driver( "123456789", "George", "Washington", DateTime.Parse( "2/22/1732" ) );
                                            Driver.NewAddress(123, "Anywhere St.", "Mt. Vernon", "VA", "12345");
                                        };

        private Because of = () =>
                                 {
                                     using (var context = Context.CreateFor(Driver))
                                     {
                                         context.PublishOnCommit<DriverGenericEvent>(x =>
                                                                                         {
                                                                                             x.EventId = EventId;
                                                                                         });
                                         context.PublishOnCommit<DriverGenericEventChild>(x =>
                                                                                              {
                                                                                                  x.EventId = EventId;
                                                                                              });
                                     }
                                 };

        private It should_have_heard_one_event_for_DriverGenericEvent = () =>
                                                                            {
                                                                                var dlist = new List<IEventListener>();
                                                                                Manager.Listeners.TryGetValue(typeof(DriverGenericEvent), out dlist);
                                                                                dlist
                                                                                    .OfType<StaticDriverEventListener>()
                                                                                    .First()
                                                                                    .Events
                                                                                    .OfType<DriverGenericEvent>()
                                                                                    .Where(x => x is DriverGenericEventChild == false)
                                                                                    .Count().ShouldEqual( 1 );
                                                                            };
        private It should_have_heard_the_DriverGenericEvent_with_the_correct_eventid = () =>
                                                                                           {
                                                                                               var dlist = new List<IEventListener>();
                                                                                               Manager.Listeners.TryGetValue(typeof(DriverGenericEvent), out dlist);
                                                                                               dlist
                                                                                                   .OfType<StaticDriverEventListener>()
                                                                                                   .First()
                                                                                                   .Events
                                                                                                   .OfType<DriverGenericEvent>()
                                                                                                   .Where(x => x is DriverGenericEventChild == false)
                                                                                                   .First()
                                                                                                   .EventId.ShouldEqual( EventId );
                                                                                           };
        private It should_have_heard_one_event_for_DriverGenericEventChild = () =>
                                                                                 {
                                                                                     var dlist = new List<IEventListener>();
                                                                                     Manager.Listeners.TryGetValue(typeof(DriverGenericEventChild), out dlist);
                                                                                     dlist.OfType<StaticDriverEventListener>().First().Events.OfType<DriverGenericEventChild>().Count().ShouldEqual( 1 );
                                                                                 };
        private It should_have_heard_the_DriverGenericEventChild_with_the_correct_eventid = () =>
                                                                                                {
                                                                                                    var dlist = new List<IEventListener>();
                                                                                                    Manager.Listeners.TryGetValue(typeof(DriverGenericEventChild), out dlist);
                                                                                                    dlist.OfType<StaticDriverEventListener>().First().Events.OfType<DriverGenericEventChild>().First().EventId.ShouldEqual( EventId );
                                                                                                };
    }
}