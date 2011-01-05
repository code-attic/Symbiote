/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Symbiote.Core;
using Symbiote.Core.Impl.Log.Impl;
using Symbiote.Log4Net.Impl;
using LogManager=Symbiote.Core.Impl.Log.LogManager;

namespace Symbiote.Log4Net
{
    public static class Log4NetAssimilation
    {
        private static IAssimilate Log4Net(this IAssimilate assimilate)
        {
            assimilate
                .Dependencies(x =>
                                  {
                                      x.For<ILogProvider>().Use<Log4NetProvider>();
                                      x.For<ILogger>().Use<Log4NetLogger>();
                                  });
            LogManager.Initialized = true;
            return assimilate;
        }

        public static IAssimilate LoadLog4NetConfig(this IAssimilate assimilate, string configFile)
        {
            assimilate.Log4Net();
            if(!File.Exists(configFile))
                throw new Exception(string.Format("There was no log4net configuration file found at {0}", configFile));
            XmlConfigurator.Configure(new FileInfo(configFile));
            return assimilate;
        }

        public static IAssimilate AddFileLogger<TLog>(this IAssimilate assimilate, Action<FileConfigurator> configure)
        {
            assimilate.Configure<TLog, FileConfigurator, FileAppender>(configure);
            return assimilate;
        }

        public static IAssimilate AddConsoleLogger<TLog>(this IAssimilate assimilate, Action<ConsoleConfigurator> configure)
        {
            assimilate.Configure<TLog, ConsoleConfigurator, ConsoleAppender>(configure);
            return assimilate;
        }

        public static IAssimilate AddRollingFileLogger<TLog>(this IAssimilate assimilate, Action<RollingConfiguration> configure)
        {
            assimilate.Configure<TLog, RollingConfiguration, RollingFileAppender>(configure);
            return assimilate;
        }

        public static IAssimilate AddColorConsoleLogger<TLog>(this IAssimilate assimilate, Action<ColorConsoleConfigurator> configure)
        {
            assimilate.Configure<TLog, ColorConsoleConfigurator, ColoredConsoleAppender>(configure);
            return assimilate;
        }

        private static void Configure<TLog, TConfigurator, TAppender>(this IAssimilate assimilate, Action<TConfigurator> action)
            where TConfigurator : AppenderConfigurator<TAppender, TConfigurator>, new()
            where TAppender : AppenderSkeleton, new()
        {
            string name = typeof (TLog).FullName;
            assimilate.Log4Net();
            var configurator = new TConfigurator();
            action(configurator);
            configurator.Activate();
            var log = log4net.LogManager.GetLogger(name);
            var logger = log.Logger as Logger;
            logger.AddAppender(configurator.Appender);
            logger.Repository.Configured = true;
        }
    }
}
