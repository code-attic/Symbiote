using System.Collections.Generic;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;
using Symbiote.Core.Hashing;

namespace Symbiote.Jackalope.Config
{
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
}