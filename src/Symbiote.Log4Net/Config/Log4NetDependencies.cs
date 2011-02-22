using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Core.Log.Impl;
using Symbiote.Log4Net.Impl;

namespace Symbiote.Log4Net.Config
{
    public class Log4NetDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            return container =>
                       {
                           container.For<ILogProvider>().Use<Log4NetProvider>();
                           container.For<ILogger>().Use<Log4NetLogger>();
                       };
        }
    }
}