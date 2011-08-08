using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Daemon.BootStrap;
using Symbiote.Daemon.Host;
using Symbiote.Daemon.Installation;
using System.Diagnostics;

namespace Symbiote.Daemon
{
    public class DaemonDependencies : IDefineDependencies
    {
        public Action<DependencyConfigurator> Dependencies()
        {
            Type hostType = null;
            var canHazMono = Type.GetType("Mono.Runtime") != null;
            if( canHazMono )
            {
                // Apparently the Mono runtime behaves differently on Ubuntu vs. Windows
                // On Windows an interactive user session runs under 'mono'
                // but under Ubuntu it runs under the actual assembly name
                var domain = System.IO.Path.GetFileNameWithoutExtension( AppDomain.CurrentDomain.FriendlyName );
				var processName = Process.GetCurrentProcess().ProcessName;
                hostType = processName == "mono" || processName == domain
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