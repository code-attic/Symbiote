using Machine.Specifications;
using Symbiote.Core.Futures;

namespace Core.Tests.Utility
{
    public class when_using_future_coroutine_on_failure
        : with_callback
    {
        private Because of = () =>
        {
            expected = -1;
            waitFor = 10;
            result = Future.Of<int>(GetResult).WaitFor(1).OnFailure( () => expected );
        };

        private It should_get_failure_value = () => result.ShouldEqual(expected);
    }
}