using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Symbiote.Core.DI;
using Topshelf.Configuration;
using Topshelf.Configuration.Dsl;
using Symbiote.Core.Extensions;

namespace Symbiote.Daemon
{
    public class DaemonConfiguration
    {
        private string _name;
        private string _displayName;
        private string _description;
        private string[] _arguments;
        private bool _asLocalSystem = true;
        private string _runAs = "";
        private string _password = "";
        private List<Type> _daemons = new List<Type>();
        protected IDependencyAdapter dependencyAdapter { get; set; }

        public string[] CommandLineArgs
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        public string ServiceName { get { return _name; } }

        public RunConfiguration RunConfig { get; set; }

        public DaemonConfiguration Name(string name)
        {
            _name = name;
            return this;
        }

        public DaemonConfiguration DisplayName(string displayName)
        {
            _displayName = displayName;
            return this;
        }

        public DaemonConfiguration Description(string description)
        {
            _description = description;
            return this;
        }

        public DaemonConfiguration Arguments(string[] arguments)
        {
            _arguments = arguments;
            return this;
        }

        public DaemonConfiguration AsAccount(string user, string password)
        {
            _runAs = user;
            _password = password;
            _asLocalSystem = false;
            return this;
        }

        public DaemonConfiguration IncludeDaemon<TDaemon>()
            where TDaemon : class, IDaemon
        {
            _daemons.Add(typeof(TDaemon));
            return this;
        }

        internal RunConfiguration GetTopShelfConfiguration()
        {
            RunConfig = RunConfig ?? RunnerConfigurator.New(ConfigureServices);
            return RunConfig;
        }

        protected void ConfigureServices(IRunnerConfigurator config)
        {
            var daemonTypes = dependencyAdapter
                .GetTypesRegisteredFor<IDaemon>();

           
            config.SetServiceName(_name);
            config.SetDisplayName(_displayName);
            config.SetDescription(_description);

            if(!_asLocalSystem)
                config.RunAs(_runAs, _password);

            daemonTypes.ForEach(x => ConfigureService(config, x));
        }

        protected void ConfigureService(IRunnerConfigurator config, Type serviceType)
        {

            config.ConfigureService<IDaemon>(service =>
            {
                service.HowToBuildService(builder => ServiceLocator.Current.GetInstance(serviceType, builder));
                service.WhenStarted(x => x.Start());
                service.WhenStopped(x => x.Stop());
            });
        }

        public DaemonConfiguration(IDependencyAdapter dependencyAdapter)
        {
            this.dependencyAdapter = dependencyAdapter;
        }
    }
}