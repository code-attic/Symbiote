using System.Threading;
using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility
{
    public class when_using_future_coroutine_invoked_immediately
        : with_callback
    {

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