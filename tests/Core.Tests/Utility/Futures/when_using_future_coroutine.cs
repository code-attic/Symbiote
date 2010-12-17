using System;
using Machine.Specifications;
using Symbiote.Core.Utility;

namespace Core.Tests.Utility
{
    public class when_using_future_coroutine
    {
        protected static int result { get; set; }

        public static void GetA5(Action<int> callback)
        {
            int val = 0;
            Random rnd = new Random();
            while (val != 5)
            {
                val = rnd.Next(0, 10);
            }
            callback(val);
        }

        private Because of = () =>
        {
            result = Future.Of<int>(GetA5).WaitFor( 1 );
        };
        
        private It should_get_result_in_time = () => result.ShouldEqual(5);
    }
}