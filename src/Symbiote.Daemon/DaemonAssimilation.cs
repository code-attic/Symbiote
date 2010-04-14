using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using StructureMap;
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
            var daemonConfiguration = new DaemonConfiguration();
            config(daemonConfiguration);
            assimilate.Dependencies(x => x.For<DaemonConfiguration>().Use(daemonConfiguration));
            return assimilate;
        }

        public static void RunDaemon(this IAssimilate assimilate)
        {
            var configuration = ObjectFactory.GetInstance<DaemonConfiguration>();
            var topshelfConfig = configuration.GetTopShelfConfiguration();
            if (configuration == null)
                throw new Exception("Please configure your service first with the Daemon<> call before attempting to run this service.");

            Runner.Host(topshelfConfig, configuration.CommandLineArgs);
        }
    }
}
