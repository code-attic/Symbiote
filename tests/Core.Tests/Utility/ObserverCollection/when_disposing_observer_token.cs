using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Core.Tests.Utility.ObserverCollection
{
    public class when_disposing_observer_token : with_observer
    {
        private Because of = () => 
        {
            using( var token = Observers.AddObserver( Observer ) )
            {
                Observers.OnEvent( "hello" );
                Observers.OnError( new Exception( "this is fun" ) );
                Observers.OnComplete();
            }
        };

        private It should_not_have_observers_in_collection = () => Observers.Count.ShouldEqual( 0 );
        private It observer_should_have_received_on_next = () => Observer.GotEvent.ShouldBeTrue();
        private It observer_should_have_received_on_error = () => Observer.GotError.ShouldBeTrue();
        private It observer_should_have_received_on_complete = () => Observer.GotCompletion.ShouldBeTrue();
    }
}
