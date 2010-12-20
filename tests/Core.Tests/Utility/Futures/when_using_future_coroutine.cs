using System;
using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility
{
    public class when_using_future_coroutine
    {
        protected static int result { get; set; }
        protected static int waitFor { get; set; }
        protected static int expected { get; set; }

        public static void GetResult(Action<int> callback)
        {
            int val = 5;
            Thread.Sleep( waitFor - 1 );
            callback(val);
        }

        private Because of = () =>
        {
            expected = 5;
            waitFor = 3;
            result = Future.Of<int>(GetResult).WaitFor( waitFor );
        };
        
        private It should_get_result_in_time = () => result.ShouldEqual(expected);
    }

    public class when_using_future_coroutine_on_failure
    {
        protected static int result { get; set; }
        protected static int waitFor { get; set; }
        protected static int expected { get; set; }

        public static void GetResult(Action<int> callback)
        {
            int val = 5;
            Thread.Sleep(waitFor - 1);
            callback(val);
        }

        private Because of = () =>
        {
            expected = -1;
            waitFor = 10;
            result = Future.Of<int>(GetResult).WaitFor(1).OnFailure( () => expected );
        };

        private It should_get_failure_value = () => result.ShouldEqual(expected);
    }

    public class when_using_future_coroutine_invoked_immediately
    {
        protected static int result { get; set; }
        protected static int waitFor { get; set; }
        protected static int expected { get; set; }

        public static void GetResult(Action<int> callback)
        {
            int val = 5;
            Thread.Sleep(waitFor - 1);
            callback(val);
        }

        private Because of = () =>
        {
            expected = 5;
            waitFor = 10;
            var futureCallback = Future.Of<int>(GetResult).WaitFor(1).OnFailure(() => expected).Now();
            Thread.Sleep( waitFor );
            result = futureCallback;
        };

        private It should_get_failure_value = () => result.ShouldEqual(expected);
    }
}