using Microsoft.Practices.ServiceLocation;
using Topshelf.Configuration.Dsl;
using Topshelf.Shelving;

namespace Symbiote.Daemon
{
    public abstract class BootstrappedDaemon<T>
        : IDaemon, IBootstrapped, Bootstrapper<T> where T : IDaemon, IBootstrapped
    {
        public void InitializeHostedService(IServiceConfigurator<T> cfg)
        {
            cfg.HowToBuildService(n => ServiceLocator.Current.GetInstance<T>());
            cfg.WhenStarted(s =>
                                {
                                    s.Initialize();
                                    s.Start();
                                });
            cfg.WhenStopped(s => s.Stop());       
        }

        public abstract void Start();
        public abstract void Stop();
        public abstract void Initialize();
    }
}