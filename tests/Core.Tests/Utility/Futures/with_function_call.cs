using System.Threading;

namespace Core.Tests.Utility
{
    public class with_function_call
    {
        protected static int result { get; set; }
        protected static int waitFor { get; set; }
        protected static int expected { get; set; }

        public static int GetResult()
        {
            int val = 5;
            Thread.Sleep(waitFor - 1);
            return val;
        }
    }
}