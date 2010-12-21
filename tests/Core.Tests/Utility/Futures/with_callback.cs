using System;
using System.Threading;

namespace Core.Tests.Utility
{
    public class with_callback
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
    }
}