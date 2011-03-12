using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Daemon.BootStrap;
using Symbiote.Daemon.Host;
using Symbiote.Daemon.Installation;
using System.Diagnostics;

namespace Symbiote.Daemon
{
    public class DaemonDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            Type hostType = null;
            var canHazMono = Type.GetType("Mono.Runtime") != null;
            if( canHazMono )
            {
                hostType = Process.GetCurrentProcess().ProcessName == "mono"
                    ? typeof(ConsoleHost)
                    : typeof(DaemonHost);                
            }
            else
            {
                hostType = Environment.UserInteractive
                    ? typeof(ConsoleHost)
                    : typeof(DaemonHost);
            }
			
            var daemonConfiguration = new DaemonConfigurator();
            return container => 
                       { 
                           container.For<DaemonConfiguration>().Use( daemonConfiguration.Configuration );
                           container.For<IServiceCoordinator>().Use<ServiceCoordinator>();
                           container.For<ICheckPermission>().Use<CredentialCheck>();
                           container.For(typeof(ServiceController<>)).Use(typeof(ServiceController<>));
                           container.For<IHost>().Use(hostType);
                           container.For<IBootStrapper>().Use<NulloBootStrapper>().AsSingleton();
                       };
        }
    }
}