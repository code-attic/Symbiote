using System;
using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility
{
    public class when_using_future_func_with_retries
        : with_function_call
    {
        private Because of = () =>
        {
            expected = 5;
            waitFor = 2;
            result = new FutureResult<int>(GetResult).WaitFor(1).MaxRetries( 10 );
        };
        
        private It should_get_result_in_time = () => result.ShouldEqual(5);
    }
}