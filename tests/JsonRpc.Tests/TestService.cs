using System;

namespace JsonRpc.Tests
{
    public class TestService : ITestService
    {
        public static string Arg1 { get; set; }

        public void OneArgCall(string arg1)
        {
            Arg1 = arg1;
        }

        public bool TwoArgCall(DateTime date, Guid id)
        {
            return true;
        }

        public ComplexReturn ComplexCall(ComplexArg arg)
        {
            return new ComplexReturn() { Date = arg.Date, Id = arg.Id, Property1 = arg.Property1 };
        }
    }
}