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
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using Topshelf;
using Topshelf.Configuration;
using Topshelf.Configuration.Dsl;

namespace Symbiote.Daemon
{
    public static class DaemonAssimilation
    {
        public static IAssimilate Daemon(this IAssimilate assimilate, Action<DaemonConfiguration> config)
        {
            assimilate.Dependencies(x => x.Scan(s =>
                                                    {
                                                        s.TheCallingAssembly();
                                                        s.AssembliesFromApplicationBaseDirectory();
                                                        s.AddAllTypesOf<IDaemon>();
                                                    }));
            
            var daemonConfiguration = ServiceLocator.Current.GetInstance<DaemonConfiguration>();
            config(daemonConfiguration);
            assimilate.Dependencies(x =>
                                        {
                                            x.For<DaemonConfiguration>().Use(daemonConfiguration);
                                            x.For<RunConfiguration>().Use(daemonConfiguration.GetTopShelfConfiguration());
                                        });
            return assimilate;
        }

        public static void RunDaemon(this IAssimilate assimilate)
        {
            var configuration = ServiceLocator.Current.GetInstance<DaemonConfiguration>();
            var topshelfConfig = configuration.GetTopShelfConfiguration();
            if (configuration == null)
                throw new Exception("Please configure your service first with the Daemon<> call before attempting to run this service.");
            Runner.Host(topshelfConfig, configuration.CommandLineArgs);
        }
    }
}
