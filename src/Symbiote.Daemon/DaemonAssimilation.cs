using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core;
using StructureMap;
using Topshelf;

namespace Symbiote.Daemon
{
    public static class DaemonAssimilation
    {
        public static IAssimilate Daemon<TDaemon>(this IAssimilate assimilate, Action<DaemonConfiguration<TDaemon>> config)
            where TDaemon : class, IDaemon
        {
            var daemonConfiguration = new DaemonConfiguration<TDaemon>();
            config(daemonConfiguration);
            assimilate
                .Dependencies(x =>
                                  {
                                      // Jeremy Miller, you are a genius!
                                      // Behold a late-bound, singleton dependency!
                                      x.For<TDaemon>().Singleton().Use(
                                          c => c.GetInstance<TDaemon>(daemonConfiguration.ServiceName));
                                      x.For<DaemonConfiguration<TDaemon>>().Use(daemonConfiguration);
                                  });
            return assimilate;
        }

        public static void RunDaemon<TDaemon>(this IAssimilate assimilate)
            where TDaemon : class, IDaemon
        {
            var configuration = ObjectFactory.GetInstance<DaemonConfiguration<TDaemon>>();

            if (configuration == null)
                throw new Exception("Please configure your service first with the ConfigureService call before attempting to run this service.");

            Runner.Host(
                configuration.GetTopShelfConfiguration(),
                configuration.CommandLineArgs);
        }
    }
}
