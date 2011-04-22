using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Machine.Specifications;

namespace Core.Tests.Fibers
{
    public class when_writing_high_volume_to_single_mailbox
        : with_director
    {
        private static List<int> numbers;
        private static Stopwatch watch;
        private static int total = 1000000;

        private Because of = () =>
            {
                numbers = Enumerable.Range( 1, total ).ToList();

                watch = Stopwatch.StartNew();

                numbers.ForEach( x => director.Send( x ) );

                watch.Stop();
            };

        private It should_finish_in_1_second = () => 
                                               watch.ElapsedMilliseconds.ShouldBeLessThanOrEqualTo( 1000 );
        private It should_complete_in_order = () => 
                                              Values[""].ShouldEqual( numbers );
        private It should_have_correct_count = () => Values[""].Count().ShouldEqual( total );
    }
}