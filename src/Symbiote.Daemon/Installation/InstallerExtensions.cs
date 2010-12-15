using System.Linq;
using System.ServiceProcess;
using Microsoft.Win32;

namespace Symbiote.Daemon.Installation
{
    public static class InstallerExtensions
    {
        private static readonly string SYSTEM = "System";
        private static readonly string CURRENT_CONTROL_SET = "CurrentControlSet";
        private static readonly string SERVICES = "Services";
        private static readonly string DESCRIPTION = "Description";

        public static ServiceInstaller GetServiceInstaller(this DaemonConfiguration configuration)
        {
            return new ServiceInstaller()
            {
                ServiceName = configuration.Name,
                Description = configuration.Description,
                DisplayName = configuration.DisplayName,
                StartType = configuration.StartMode
            };
        }

        public static ServiceProcessInstaller GetProcessInstaller(this DaemonConfiguration configuration)
        {
            return new ServiceProcessInstaller()
            {
                Account = configuration.PrincipalType,
                Username = configuration.Principal,
                Password = configuration.Password
            };
        }

        public static void UpdateRegistry(this DaemonConfiguration configuration)
        {
            using (RegistryKey system = Registry.LocalMachine.OpenSubKey( SYSTEM ))
            using (RegistryKey currentControlSet = system.OpenSubKey( CURRENT_CONTROL_SET ))
            using(RegistryKey services = currentControlSet.OpenSubKey( SERVICES ))
            using(RegistryKey service = services.OpenSubKey( configuration.Name, true ))
            {
                service.SetValue( DESCRIPTION, configuration.Description );
            }
        }

        public static bool Installed(this DaemonConfiguration configuration)
        {
            return ServiceController
                .GetServices()
                .Any( x => x.ServiceName == configuration.Name );
        }
    }
}