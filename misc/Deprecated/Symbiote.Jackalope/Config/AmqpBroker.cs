using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;

namespace Symbiote.Jackalope.Config
{
    public class AmqpBroker
        : IBroker
    {
        private IConnection _connection;
        private const string DEFAULT = "default";
        protected IDictionary<string, IAmqpServerConfiguration> Servers { get; set; }
        protected ConnectionFactory Factory { get; set; }
        protected ConnectionDistributor Distributor { get; set; }
        protected IConnection Connection
        {
            get
            {
                _connection = _connection ?? Distributor.Connections.First();
                return _connection;
            }
            set { _connection = value; }
        }
        public string Name { get; set; }
        public string Protocol { get { return Servers.First().Value.Protocol; } }
        public bool LoadBalanced { get; set; }

        public void AddServer(IAmqpServerConfiguration server)
        {
            if(Servers.Count > 1)
                LoadBalanced = true;

            Servers.Add(server.Address, server);
            try
            {
                var connection = server.GetConnection();
                if(!LoadBalanced)
                {
                    Connection = connection;
                }
                Distributor.AddNode(server.Address, connection);    
            }
            catch (Exception ex)
            {
                "An exception occurred attempting to connect to the amqp server at {0}:{1}"
                    .ToError<IBus>(server.Address, server.Port);
            }
        }

        public IConnection GetConnection()
        {
            if(LoadBalanced)
            {
                return Distributor.Connections.First();
            }
            else
            {
                return Connection;
            }
        }

        public IEnumerable<IConnection> GetConnections()
        {
            return Distributor.Connections;
        }

        public IConnection GetBalancedConnection(string id)
        {
            if(LoadBalanced)
            {
                return Distributor.GetNode(id);
            }
            else
            {
                return Connection;
            }
        }

        public void Init(string name)
        {
            Name = name;
            Distributor = new ConnectionDistributor();
            Factory = new ConnectionFactory();
            Servers = new Dictionary<string, IAmqpServerConfiguration>();
        }

        public AmqpBroker(string name)
        {
            Init(name);
        }

        public AmqpBroker()
        {
            Init(DEFAULT);
        }
    }
}