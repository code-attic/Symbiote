using System;
using System.Linq;
using RabbitMQ.Client;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope.Config;

namespace Symbiote.Jackalope.Impl.Server
{
    public class ConnectionManager : IConnectionManager
    {
        private IAmqpConfigurationProvider _configurationProvider;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;

        public IConnection GetConnection()
        {
            if (_connection == null || !_connection.IsOpen)
                OpenConnection();
            return _connection;
        }

        public string Protocol
        {
            get { return _configurationProvider.Servers[0].Protocol; }
        }

        protected void OpenConnection()
        {
            try
            {
                var serverConfiguration = _configurationProvider.Servers.First();
                var protocol = Protocols.Lookup(serverConfiguration.Protocol);
                var hostName = serverConfiguration.Address;
                var port = serverConfiguration.Port;
                _connectionFactory.UserName = serverConfiguration.User;
                _connectionFactory.Password = serverConfiguration.Password;
                _connectionFactory.VirtualHost = serverConfiguration.VirtualHost;
                _connectionFactory.Protocol = protocol;
                _connectionFactory.HostName = hostName;
                _connectionFactory.Port = port;
                _connection = _connectionFactory.CreateConnection();
                _connection.ConnectionShutdown += ConnectionShutdown;
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception occurred trying to create a connection to a RabbitMQ node :(");
            }
        }

        void ConnectionShutdown(IConnection connection, ShutdownEventArgs reason)
        {
            "The connection to the rabbitmq node is shutting down. \r\n\t Class: {0} \r\n\t Method: {1} \r\n\t Cause: {2} \r\n\t Reply {3}: {4}"
                .ToError<IBus>
                (
                    reason.ClassId,
                    reason.MethodId,
                    reason.Cause,
                    reason.ReplyCode,
                    reason.ReplyText
                );
            connection.ConnectionShutdown -= ConnectionShutdown;
            _connection = null;
            connection.Dispose();
            connection = null;
        }

        public ConnectionManager(IAmqpConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
            _connectionFactory = new ConnectionFactory();
        }


    }
}