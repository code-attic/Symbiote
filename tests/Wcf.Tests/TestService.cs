using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wcf.Tests
{
    public class TestService : ITestService
    {
        public bool TwoArgCall(DateTime date, Guid id)
        {
            return true;
        }

        public TestService()
        {
        }
    }
}
