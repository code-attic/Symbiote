using System;
using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility
{
    public class when_using_future_func
    {
        protected static int result { get; set; }

        public static int GetA5()
        {
            int val = 0;
            Random rnd = new Random();
            while ( val != 5 )
            {
                val = rnd.Next( 0, 10 );
            }

            return val;
        }

        private Because of = () =>
        {
            result = Future.Of(GetA5).WaitFor(10).MaxRetries(2).OnValue(Console.Write);
        };
        
        private It should_get_result_in_time = () => result.ShouldEqual( 5 );
    }
}
