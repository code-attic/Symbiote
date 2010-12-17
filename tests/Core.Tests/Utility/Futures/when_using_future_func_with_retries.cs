using System;
using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility
{
    public class when_using_future_func_with_retries
    {
        protected static int result { get; set; }

        public static int GetA5()
        {
            int val = 0;
            Random rnd = new Random(DateTime.UtcNow.Millisecond);
            while (val != 5)
            {
                val = rnd.Next(0, 10);
            }

            return val;
        }

        private Because of = () =>
        {
            result = new FutureResult<int>(GetA5).WaitFor(1).MaxRetries( 10 );
        };
        
        private It should_get_result_in_time = () => result.ShouldEqual(5);
    }
}