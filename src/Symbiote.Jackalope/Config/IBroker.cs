using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;
using Symbiote.Core.Hashing;
using System.Linq;

namespace Symbiote.Jackalope.Config
{
    public interface IBroker
    {
        string Name { get; set; }
        string Protocol { get; }
        void AddServer(IAmqpServerConfiguration server);
        IConnection GetConnection();
        IConnection GetBalancedConnection(string id);
    }

    public class ConnectionDistributor
        : Distributor<IConnection>
    {
        public IEnumerable<IConnection> Connections
        {
            get { return Nodes.Values; }
        }

        public override void AddNode(string name, IConnection node)
        {
            base.AddNode(name, node);
            node.ConnectionShutdown += NodeShutdown;
        }

        void NodeShutdown(IConnection connection, ShutdownEventArgs reason)
        {
            RemoveNode(connection.Endpoint.HostName);
            "The connection to the rabbitmq node is shutting down. \r\n\t Class: {0} \r\n\t Method: {1} \r\n\t Cause: {2} \r\n\t Reply {3}: {4}"
                .ToError<IBus>
                (
                    reason.ClassId,
                    reason.MethodId,
                    reason.Cause,
                    reason.ReplyCode,
                    reason.ReplyText
                );
        }

        public override void RemoveNode(string name)
        {
            IConnection node = null;
            if(Nodes.TryGetValue(name, out node))
            {
                node.ConnectionShutdown -= NodeShutdown;
            }
            base.RemoveNode(name);
        }

        public ConnectionDistributor() : base(2000)
        {
        }
    }

    public class AmqpBroker
        : IBroker
    {
        private const string DEFAULT = "default";
        protected IDictionary<string, IAmqpServerConfiguration> Servers { get; set; }
        protected ConnectionFactory Factory { get; set; }
        protected ConnectionDistributor Distributor { get; set; }
        protected IConnection Connection { get; set; }
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
                    Connection = connection;
                Distributor.AddNode(server.Address, connection);    
            }
            catch (Exception)
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