namespace Symbiote.Jackalope.Config
{
    public class AmqpServer : IAmqpServerConfiguration
    {
        public string Protocol { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }

        public void SetDefaults()
        {
            Protocol = "AMQP_0_8";
            User = "guest";
            Password = "guest";
            VirtualHost = "/";
            Address = "localhost";
            Port = 5672;
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