using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;

namespace Core.Tests.Log
{
    public class when_logging_without_initialization : with_log_manager
    {
        private Because of = () => 
        { 
            LogManager.Log<string>( LogLevel.Debug, "test" );
        };

        private It should_have_called_null_logger = () => NullLogger.Called.ShouldBeTrue();
    }
}
