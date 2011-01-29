using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Tests.Domain.Listeners;
using Core.Tests.Domain.Model;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.UnitOfWork;

namespace Core.Tests.UnitOfWork
{
    public class with_EventListenerManager : with_assimilation
    {
        public static IEventListenerManager Manager;

        private Establish context = () =>
                                        {
                                            Manager = Assimilate.GetInstanceOf<IEventListenerManager>();
                                        };
    }

    public class when_testing_Static_Event_Listeners : with_EventListenerManager
    {
        private It should_have_six_entries_in_dictionary = () =>
                                                                 {
                                                                     Manager.Listeners.Count.ShouldEqual(6);
                                                                 };
        private It should_have_VehicleGenericEvent_in_dictionary = () => Manager.Listeners.ContainsKey(typeof( VehicleGenericEvent )).ShouldBeTrue();
        private It should_have_DriverGenericEvent_in_dictionary  = () => Manager.Listeners.ContainsKey(typeof( DriverGenericEvent )).ShouldBeTrue();
        private It should_have_DriverGenericEventChild_in_dictionary = () => Manager.Listeners.ContainsKey(typeof(DriverGenericEventChild)).ShouldBeTrue();
        private It should_have_VehicleGenericEventChild_in_dictionary = () => Manager.Listeners.ContainsKey(typeof(VehicleGenericEventChild)).ShouldBeTrue();
        private It should_have_StaticListenerForEverything_in_dictionary_for_all_events = () =>
                                                                             {
                                                                                 Manager.Listeners.SelectMany(s => s.Value).Count(x => x is StaticListenerForEverything).ShouldEqual(6);
                                                                             };
        private It should_have_StaticDriverEventListener_in_dictionary_for_DriverGenericEvent = () =>
                                                                             {
                                                                                 var dlist = new List<IEventListener>();
                                                                                 Manager.Listeners.TryGetValue(typeof(DriverGenericEvent), out dlist);
                                                                                 dlist.Count(x => x.GetType() == typeof(StaticDriverEventListener)).ShouldEqual( 1 );
                                                                             };
        private It should_have_StaticDriverEventListener_in_dictionary_for_DriverGenericEventChild = () =>
                                                                             {
                                                                                 var dlist = new List<IEventListener>();
                                                                                 Manager.Listeners.TryGetValue(typeof(DriverGenericEventChild), out dlist);
                                                                                 dlist.Count(x => x.GetType() == typeof(StaticDriverEventListener)).ShouldEqual( 1 );
                                                                             };
        private It should_have_StaticVehicleEventListener_in_dictionary = () =>
                                                                             {
                                                                                 var dlist = new List<IEventListener>();
                                                                                 Manager.Listeners.TryGetValue(typeof(VehicleGenericEvent), out dlist);
                                                                                 dlist.Count(x => x.GetType() == typeof(StaticVehicleEventListener)).ShouldEqual(1);
                                                                             };
    }

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
                                     var provider = Assimilate.GetInstanceOf<IContextProvider>();
                                     using (var context = provider.GetContext(Driver))
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
                                                             .Count()
                                                             .ShouldEqual(1);
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
                                                             .EventId
                                                             .ShouldEqual(EventId);
                                                     };
        private It should_have_heard_one_event_for_DriverGenericEventChild = () =>
                                                     {
                                                         var dlist = new List<IEventListener>();
                                                         Manager.Listeners.TryGetValue(typeof(DriverGenericEventChild), out dlist);
                                                         dlist.OfType<StaticDriverEventListener>().First().Events.OfType<DriverGenericEventChild>().Count().ShouldEqual(1);
                                                     };
        private It should_have_heard_the_DriverGenericEventChild_with_the_correct_eventid = () =>
                                                     {
                                                         var dlist = new List<IEventListener>();
                                                         Manager.Listeners.TryGetValue(typeof(DriverGenericEventChild), out dlist);
                                                         dlist.OfType<StaticDriverEventListener>().First().Events.OfType<DriverGenericEventChild>().First().EventId.ShouldEqual(EventId);
                                                     };
    }

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
            var provider = Assimilate.GetInstanceOf<IContextProvider>();
            using (var context = provider.GetContext(Vehicle))
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
            dlist.OfType<StaticVehicleEventListener>().First().Events.Count.ShouldEqual(1);
        };
        private It should_have_heard_the_event_with_the_correct_eventid = () =>
        {
            var dlist = new List<IEventListener>();
            Manager.Listeners.TryGetValue(typeof(VehicleGenericEvent), out dlist);
            dlist.OfType<StaticVehicleEventListener>().First().Events[0].EventId.ShouldEqual(EventId);
        };
    }
}
