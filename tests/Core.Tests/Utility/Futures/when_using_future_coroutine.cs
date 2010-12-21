using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility
{
    public class when_using_future_coroutine
        : with_callback
    {
        private Because of = () =>
        {
            expected = 5;
            waitFor = 3;
            result = Future.Of<int>(GetResult).WaitFor( waitFor );
        };
        
        private It should_get_result_in_time = () => result.ShouldEqual(expected);
    }
}