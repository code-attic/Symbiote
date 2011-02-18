using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Futures;

namespace Core.Tests.Utility.Futures
{
    public class when_using_future_async_call_in_loop
    {
        public static List<int> results = new List<int>();
        public static int calls;

        private Because of = () => 
                                 {
                                     var fakeOut = new AsyncFakeOut();
                                     Future.Of( 
                                         x => fakeOut.BeginRead( x ),
                                         x => 
                                             {
                                                 calls++;
                                                 var result = fakeOut.EndRead( x );
                                                 return result;
                                             })
                                         .WaitFor( 5 )
                                         .OnValue( x => results.Add( x ) )
                                         .LoopWhile( () => results.Count < 5 )
                                         .Start();

                                     Thread.Sleep( 100 );
                                 };

        private It should_produce_ordered_results = () => results.ShouldEqual( new [] { 1, 2, 3, 4, 5 }.ToList() );
        private It should_have_5_results = () => results.Count.ShouldEqual( 5 );
        private It should_make_call_5_times = () => calls.ShouldEqual( 5 );
    }
}