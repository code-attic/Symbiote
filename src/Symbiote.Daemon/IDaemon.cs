using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Topshelf.Configuration.Dsl;
using Topshelf.Shelving;

namespace Symbiote.Daemon
{
    public abstract class BootstrappedDaemon<T>
        : IDaemon, Bootstrapper<T> where T : IDaemon
    {
        public void InitializeHostedService(IServiceConfigurator<T> cfg)
        {
            cfg.HowToBuildService(n => ServiceLocator.Current.GetInstance<T>());
            cfg.WhenStarted(s => s.Start());
            cfg.WhenStopped(s => s.Stop());       
        }

        public abstract void Start();
        public abstract void Stop();
    }

    public interface IDaemon
    {
        void Start();
        void Stop();
    }
}
