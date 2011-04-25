using System;
using Machine.Specifications;
using Symbiote.Core;
using Symbiote.Core.Collections;
using Symbiote.Core.Log;
using Symbiote.Core.Log.Impl;

namespace Core.Tests.Log
{
    public class with_log_manager : with_assimilation
    {
        protected static ILogManager LogManager;

        private Establish context = () => { LogManager = Assimilate.GetInstanceOf<ILogManager>(); };

        Cleanup reset = () => 
        {
            //(LogManager as LogManager).Initialized = false;
            //(LogManager as LogManager).Loggers = new ExclusiveConcurrentDictionary<Type, ILogger>();
            NullLogger.Called = false;
        };
    }
}