using System;
using RabbitMQ.Client;

namespace Symbiote.Jackalope.Config
{
    public class AmqpServer : IAmqpServerConfiguration
    {
        protected ConnectionFactory _factory { get; set; }
        public string Broker { get; set; }
        public string Protocol { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        protected ConnectionFactory Factory
        {
            get
            {
                _factory = _factory ?? new ConnectionFactory()
                                           {
                                               HostName = Address,
                                               Port = Port,
                                               UserName = User,
                                               Password = Password,
                                               VirtualHost = VirtualHost,
                                               Protocol = Protocols.Lookup(Protocol)
                                           };
                return _factory;
            }
        }

        public void SetDefaults()
        {
            Broker = "default";
            Protocol = "AMQP_0_9_1";
            User = "guest";
            Password = "guest";
            VirtualHost = "/";
            Address = "localhost";
            Port = 5672;
        }

        public IConnection GetConnection()
        {
            return Factory.CreateConnection();
        }

        public AmqpServer(string address, int port)
        {
            SetDefaults();
            Address = address;
            Port = port;
        }

        public AmqpServer()
        {
            SetDefaults();
        }
    }
}