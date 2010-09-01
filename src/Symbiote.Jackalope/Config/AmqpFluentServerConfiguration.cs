namespace Symbiote.Jackalope.Config
{
    public class AmqpFluentServerConfiguration
    {
        internal AmqpServer Server { get; set; }

        public AmqpFluentServerConfiguration Address(string address)
        {
            Server.Address = address;
            return this;
        }

        public AmqpFluentServerConfiguration Port(int port)
        {
            Server.Port = port;
            return this;
        }

        public AmqpFluentServerConfiguration User(string user)
        {
            Server.User = user;
            return this;
        }

        public AmqpFluentServerConfiguration Password(string password)
        {
            Server.Password = password;
            return this;
        }

        public AmqpFluentServerConfiguration VirtualHost(string virtualHost)
        {
            Server.VirtualHost = virtualHost;
            return this;
        }

        public AmqpFluentServerConfiguration AMQP08()
        {
            Server.Protocol = "AMQP_0_8";
            return this;
        }

        public AmqpFluentServerConfiguration AMQP09()
        {
            Server.Protocol = "AMQP_0_9";
            return this;
        }

        public AmqpFluentServerConfiguration AMQP091()
        {
            Server.Protocol = "AMQP_0_9_1";
            return this;
        }

        public AmqpFluentServerConfiguration()
        {
            Server = new AmqpServer();
        }
    }
}