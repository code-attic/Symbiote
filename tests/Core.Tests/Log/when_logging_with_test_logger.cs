using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log.Impl;

namespace Core.Tests.Log
{
    public class when_logging_with_test_logger : with_test_logger
    {
        private Because of = () => 
                                 { 
                                     "This is a test".ToDebug<string>();
                                     LogProvider = Assimilate.GetInstanceOf<ILogProvider>() as TestLogProvider;
                                 };

        private It should_have_sent_message_to_test_log = () => 
            ( LogProvider.Logger as TestLogger ).Content.ShouldContain( new [] {"Debug - This is a test"} );

        private It should_not_have_sent_to_null_logger = () => NullLogger.Called.ShouldBeFalse();
    }
}