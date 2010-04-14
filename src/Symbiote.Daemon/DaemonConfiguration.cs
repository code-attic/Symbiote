using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
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
        private List<Type> _daemons = new List<Type>();

        public string[] CommandLineArgs
        {
            get { return _arguments; }
            set { _arguments = value; }
        }

        public string ServiceName { get { return _name; } }

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

        public DaemonConfiguration IncludeDaemon<TDaemon>()
            where TDaemon : class, IDaemon
        {
            _daemons.Add(typeof(TDaemon));
            return this;
        }

        internal RunConfiguration GetTopShelfConfiguration()
        {
            var config = RunnerConfigurator.New(ConfigureServices);
            ObjectFactory.Inject(config.Coordinator);
            return config;
        }

        protected void ConfigureServices(IRunnerConfigurator config)
        {
            var concreteTypes = ObjectFactory.Model.InstancesOf<IDaemon>();

            if (_daemons.Count > 0)
                concreteTypes = concreteTypes.Where(x => _daemons.Contains(x.ConcreteType));

            config.SetServiceName(_name);
            config.SetDisplayName(_displayName);
            config.SetDescription(_description);
            concreteTypes.ForEach(x => ConfigureService(config, x.ConcreteType));
        }

        protected void ConfigureService(IRunnerConfigurator config, Type serviceType)
        {

            config.ConfigureService<IDaemon>(service =>
            {
                service.HowToBuildService(builder => ObjectFactory.GetInstance(serviceType));
                service.WhenStarted(x => x.Start());
                service.WhenStopped(x => x.Stop());
            });
        }

        public DaemonConfiguration()
        {
        }
    }
}