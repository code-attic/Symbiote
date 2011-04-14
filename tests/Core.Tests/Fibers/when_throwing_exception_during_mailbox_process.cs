using System.Collections.Generic;
using Machine.Specifications;

namespace Core.Tests.Fibers
{
    public class when_throwing_exception_during_mailbox_process
        : with_director_throwing_exceptions
    {
        static List<int> numbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        static List<int> results = new List<int>() { 1, 2, 4, 5, 7, 8, 10 };

        private Because of = () =>
            {
                numbers.ForEach( x => director.Send( x ) );
            };

        private It should_have_correct_result = () => Values[""].ShouldEqual( results );
        private It should_have_seven_results = () => Values[""].Count.ShouldEqual( 7 );
    }
}