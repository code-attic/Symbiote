using System.Collections.Generic;
using Symbiote.Jackalope.Config;
using Symbiote.Relax;

namespace Symbiote.Telepathy
{
    public class JackalopeServerConfiguration : DefaultCouchDocument, IAmqpServerConfiguration
    {
        public string Protocol { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public List<JackalopeEndpointConfiguration> EndPoints { get; set; }

        public void AddEndPoint(JackalopeEndpointConfiguration endpoint)
        {
            EndPoints.Add(endpoint);
        }

        public void SetDefaults()
        {
            EndPoints = new List<JackalopeEndpointConfiguration>();
            Protocol = "AMQP_0_8";
            User = "guest";
            Password = "guest";
            VirtualHost = "/";
            Address = "localhost";
            Port = 5672;
        }

        public JackalopeServerConfiguration(string address, int port)
        {

            SetDefaults();
            Address = address;
            Port = port;
        }

        public JackalopeServerConfiguration()
        {
            SetDefaults();
        }
    }
}