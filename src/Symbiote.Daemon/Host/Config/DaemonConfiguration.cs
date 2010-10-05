using System;

namespace Symbiote.Daemon
{
    public class DaemonConfiguration
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string[] Arguments { get; set; }
        public string Principal { get; set; }
        public string Password { get; set; }
        public bool AsLocal { get; set; }
        public TimeSpan StartupTimeout { get; set; }

        public DaemonConfiguration()
        {
            AsLocal = true;
            StartupTimeout = TimeSpan.FromSeconds(30);
            Arguments = new string[]{};
        }
    }
}