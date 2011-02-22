using System.Collections.Generic;
using Core.Tests.Domain.Listeners;
using Machine.Specifications;
using Symbiote.Core.UnitOfWork;
using System.Linq;

namespace Core.Tests.UnitOfWork
{
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
                                                                                                  ShouldExtensionMethods.ShouldEqual( Manager.Listeners.SelectMany(s => s.Value).Count(x => x is StaticListenerForEverything), 6);
                                                                                              };
        private It should_have_StaticDriverEventListener_in_dictionary_for_DriverGenericEvent = () =>
                                                                                                    {
                                                                                                        var dlist = new List<IEventListener>();
                                                                                                        Manager.Listeners.TryGetValue(typeof(DriverGenericEvent), out dlist);
                                                                                                        ShouldExtensionMethods.ShouldEqual( dlist.Count(x => x.GetType() == typeof(StaticDriverEventListener)), 1 );
                                                                                                    };
        private It should_have_StaticDriverEventListener_in_dictionary_for_DriverGenericEventChild = () =>
                                                                                                         {
                                                                                                             var dlist = new List<IEventListener>();
                                                                                                             Manager.Listeners.TryGetValue(typeof(DriverGenericEventChild), out dlist);
                                                                                                             ShouldExtensionMethods.ShouldEqual( dlist.Count(x => x.GetType() == typeof(StaticDriverEventListener)), 1 );
                                                                                                         };
        private It should_have_StaticVehicleEventListener_in_dictionary = () =>
                                                                              {
                                                                                  var dlist = new List<IEventListener>();
                                                                                  Manager.Listeners.TryGetValue(typeof(VehicleGenericEvent), out dlist);
                                                                                  ShouldExtensionMethods.ShouldEqual( dlist.Count(x => x.GetType() == typeof(StaticVehicleEventListener)), 1);
                                                                              };
    }
}