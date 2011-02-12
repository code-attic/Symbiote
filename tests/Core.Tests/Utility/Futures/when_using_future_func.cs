using System;
using System.Collections.Generic;
using System.IO;
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
        public static int calls;

        private Because of = () =>
                                 {

                                     waitFor = 2;
                                     expected = 5;
                                     Future.Of( () => 
                                            {
                                                calls++;
                                                return calls;
                                            } )
                                         .WaitFor( waitFor )
                                         .LoopWhile( () => results.Count < 5 )
                                         .OnValue( x => results.Add( x ) )
                                         .Start();

                                     Thread.Sleep( 200 );
                                 };

        private It should_produce_ordered_results = () => results.ShouldEqual( new [] { 1, 2, 3, 4, 5 }.ToList() );
        private It should_have_5_results = () => results.Count.ShouldEqual( 5 );
        private It should_make_call_5_times = () => calls.ShouldEqual( 5 );
    }

    public class AsyncFakeOut
    {
        private int _value = 1;

        private CallbackResult _trigger = new CallbackResult();

        public IAsyncResult BeginRead(AsyncCallback callback)
        {
            _trigger.Reset();
            Thread.Sleep( 2 );
            _trigger.Set();
            callback(_trigger);
            return _trigger;
        }

        public int EndRead(IAsyncResult result)
        {
            result.AsyncWaitHandle.WaitOne();
            return _value++;
        }
    }

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
