using System;
using Symbiote.Core;
using Symbiote.Core.DI;
using Symbiote.Daemon.BootStrap;
using Symbiote.Daemon.Host;
using Symbiote.Daemon.Installation;

namespace Symbiote.Daemon
{
    public class DaemonDependencies : IDefineStandardDependencies
    {
        public Action<DependencyConfigurator> DefineDependencies()
        {
            var hostType = Environment.UserInteractive
                               ? typeof(ConsoleHost)
                               : typeof(DaemonHost);
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