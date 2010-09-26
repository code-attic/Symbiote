using System;
using System.Linq;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;

namespace Symbiote.Jackalope.Impl.Server
{
    public class ConnectionManager : IConnectionManager
    {
        protected IAmqpConfigurationProvider Provider { get; set; }

        protected IBroker DefaultBroker { get { return Provider.Brokers["default"]; } }

        public string Protocol
        {
            get { return Provider.Brokers.Values.First().Protocol; }
        }

        public IConnection GetConnection()
        {
            return DefaultBroker.GetConnection();
        }

        public IConnection GetConnection(string brokerName)
        {
            return Provider.Brokers[brokerName].GetConnection();
        }

        public IConnection GetConnection<T>(T id)
        {
            return DefaultBroker.GetBalancedConnection(id.ToString());
        }

        public IConnection GetConnection<T>(T id, string brokerName)
        {
            return Provider.Brokers[brokerName].GetBalancedConnection(id.ToString());
        }

        public ConnectionManager(IAmqpConfigurationProvider configurationProvider)
        {
            Provider = configurationProvider;
        }
    }
}