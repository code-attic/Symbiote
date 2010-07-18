using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wcf.Tests
{
    public class TestService : ITestService
    {
        public Return TwoArgCall(DateTime date, Guid id)
        {
            return new Return() { datetime = date};
        }

        public TestService()
        {
        }
    }
}
