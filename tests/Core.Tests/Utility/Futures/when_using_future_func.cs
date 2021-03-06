﻿using Machine.Specifications;
using Symbiote.Core.Futures;

namespace Core.Tests.Utility.Futures
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
}
