using System;

namespace Core.Tests.Reflection
{
    public class TestClass
    {
        internal string val1 = "1";
        protected int val2 = 2;
        private TimeSpan val3 = TimeSpan.FromSeconds(3);
        public Guid val4 = new Guid("00000000-0000-0000-0000-000000000004");
        public ChildTestClass val5 { get; set; }

        public int GetVal2()
        {
            return val2;
        }

        public TimeSpan GetVal3()
        {
            return val3;
        }

        public TestClass()
        {
            val5 = new ChildTestClass();
        }
    }

    public class ChildTestClass
    {
        public string val5a = "child";
        public decimal val5b = 10.1m;
    }
}