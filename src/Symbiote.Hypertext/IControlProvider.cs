using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using StructureMap;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Jackalope;
using Symbiote.Jackalope.Config;
using Symbiote.Jackalope.Impl;
using Symbiote.Relax;
using Symbiote.Relax.Impl;
using Symbiote.Core.Extensions;

namespace Symbiote.Telepathy
{
    public static class TelepathyAssimilation
    {
        public static IAssimilate Hive(this IAssimilate assimilate, Action<HiveConfigurator> configurator)
        {
            var hiveConfigurator = new HiveConfigurator(assimilate);
            configurator(hiveConfigurator);
            return assimilate;
        }
    }

    public interface INodeName
    {
        string Machine { get; set; }
        string Service { get; set; }
        int Process { get; set; }
    }

    public class Registration : BaseCouchDocument<string, string>
    {
        public INodeName Node { get; set; }
    }

    public class NodeName : INodeName
    {
        public string Machine { get; set; }
        public string Service { get; set; }
        public int Process { get; set; }

        public NodeName()
        {
            Machine = Environment.MachineName;
            Process = Thread.CurrentContext.ContextID;
            var serviceType =
                ObjectFactory.Container.Model.PluginTypes.First(x => x.GetType().IsAssignableFrom(typeof (IDaemon)));
            Service = serviceType.PluginType.Name;
        }

        public override string  ToString()
        {
            return "{0}|{1}|{2}"
                .AsFormat(Machine, Service, Process);
        }

        public string ToString(string delimiter)
        {
            return DelimitedBuilder.Construct(
                new string[] {Machine, Service, Process.ToString()}, 
                delimiter);
        }
    }

    public class RegistrationMessage
    {
        public INodeName Node { get; set; }
    }

    public class HiveConfigurator
    {
        private string __controlVirtualHost = "/hive";
        private string _registryExchange = "registry";
        private string _configurationExchange = "configuration";
        private string _controlExchange = "control";
        private string _queueName = "";
        private NodeName _nodeName;
        private IBus _bus;
        private IDocumentRepository _couch;

        private IAssimilate assimilate;

        public HiveConfigurator UseCustomExchanges(string registryExchange, string configurationExchange, string controlExchange)
        {
            _registryExchange = registryExchange;
            _configurationExchange = configurationExchange;
            _controlExchange = controlExchange;

            return this;
        }

        public HiveConfigurator UseCustomNodeName(string machineName, string serviceName, int processId)
        {
            _nodeName = new NodeName() {Machine = machineName, Service = serviceName, Process = processId};
            return this;
        }

        protected List<string> GetRoutingKeys()
        {
            return new[]
                       {
                           "{0}*".AsFormat(_nodeName.Machine),
                           "{0}_{1}*".AsFormat(_nodeName.Machine, _nodeName.Service),
                           "*{1}*".AsFormat(_nodeName.Service),
                           "{0}_{1}_{2}".AsFormat(_nodeName.Machine, _nodeName.Service, _nodeName.Process),
                       }.ToList();
        }
        
        public void Start()
        {
            _bus.AddEndPoint(e => e
                .Exchange(_registryExchange, ExchangeType.topic)
                .QueueName(_queueName)
                .RoutingKeys(GetRoutingKeys().ToArray()));
            Register();
            _bus.Subscribe(_queueName, null);
        }

        protected void Register()
        {
            try
            {
                _bus.Send(_registryExchange, new RegistrationMessage() {Node = _nodeName});
            }
            catch (Exception e)
            {
                "Failed to register service with control bus due to error. The node will shut down. \r\n\t{0}"
                    .ToConsole(e);
                Environment.Exit(-1);
            }
        }

        public HiveConfigurator(IAssimilate assimilate)
        {
            this.assimilate = assimilate;
            _nodeName = new NodeName();
            _queueName = _nodeName.ToString("_");
            _couch = ObjectFactory.GetInstance<IDocumentRepository>();
            _bus = ObjectFactory.GetInstance<IBus>();
        }
    }
}
