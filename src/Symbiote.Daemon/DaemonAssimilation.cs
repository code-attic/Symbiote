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
