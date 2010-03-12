using Microsoft.Practices.ServiceLocation;
using StructureMap;
using Topshelf.Configuration;

namespace Symbiote.Daemon
{
    public class DaemonConfiguration<TDaemon>
        where TDaemon : class, IDaemon
    {
        private string _name;
        private string _displayName;
        private string _description;
        private string[] _arguments;

        public string[] CommandLineArgs
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        public string ServiceName { get { return _name; } }

        public DaemonConfiguration<TDaemon> Name(string name)
        {
            _name = name;
            return this;
        }
        public DaemonConfiguration<TDaemon> DisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }
        public DaemonConfiguration<TDaemon> Description(string description)
        {
            _description = description;
            return this;
        }

        public DaemonConfiguration<TDaemon> Arguments(string[] arguments)
        {
            _arguments = arguments;
            return this;
        }

        internal IRunConfiguration GetTopShelfConfiguration()
        {
            var serviceKey = typeof(TDaemon).FullName;

            var config = RunnerConfigurator.New(
                c =>
                    {
                        c.SetServiceName(_name);
                        c.SetDisplayName(_displayName);
                        c.SetDescription(_description);

                        c.ConfigureService<TDaemon>(
                            serviceKey,
                            service =>
                                {
                                    service.CreateServiceLocator(() => ServiceLocator.Current);
                                    service.WhenStarted(s => s.Start());
                                    service.WhenStopped(s => s.Stop());
                                });
                        c.DependencyOnMsmq();
                    });
            return config;
        }

        public DaemonConfiguration()
        {
        }
    }
}