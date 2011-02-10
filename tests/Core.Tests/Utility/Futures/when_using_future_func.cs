using System;
using System.Collections.Generic;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Futures;
using System.Linq;

namespace Core.Tests.Utility
{
    public class when_using_future_func
        : with_function_call
    {
        private Because of = () =>
        {
            waitFor = 10;
            expected = 5;
            result = Future.Of(GetResult).WaitFor(waitFor);
        };
        
        private It should_get_result_in_time = () => result.ShouldEqual( expected );
    }

    public class when_using_future_func_in_loop
        : with_function_call
    {
        public static List<int> results = new List<int>();

        private Because of = () =>
                                 {

                                     waitFor = 2;
                                     expected = 5;
                                     Future.Of( GetResult )
                                         .WaitFor( waitFor )
                                         .LoopWhile( () => results.Count < 5 )
                                         .OnValue( x => results.Add( x ) )
                                         .Start();

                                     Thread.Sleep( 20 );
                                 };

        private It should_produce_five_results = () => results.All( x => x == expected ).ShouldBeTrue();
    }
}
