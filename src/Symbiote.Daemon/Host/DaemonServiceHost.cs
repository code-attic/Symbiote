using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Daemon.Host
{
    public class DaemonServiceHost
        : IHost
    {
        readonly IServiceCoordinator ServiceCoordinator;
        readonly ServiceName Name;

        public void Start()
        {
            ServiceCoordinator.Start();
        }

        public DaemonServiceHost( IServiceCoordinator serviceCoordinator, ServiceName name )
        {
            ServiceCoordinator = serviceCoordinator;
            Name = name;
        }
    }
}
