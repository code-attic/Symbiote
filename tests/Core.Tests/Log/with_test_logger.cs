using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Log.Impl;

namespace Core.Tests.Log
{
    public class with_test_logger : with_assimilation
    {
        protected static TestLogProvider LogProvider;

        private Establish context = () => 
                                        { 
                                            Assimilate.UseTestLogAdapter( Assimilate.Assimilation );
                                        };
    }
}