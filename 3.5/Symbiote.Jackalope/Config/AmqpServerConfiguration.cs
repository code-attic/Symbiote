using System.Configuration;

namespace Symbiote.Jackalope.Config
{
    public class AmqpServerConfiguration : ConfigurationElement, IAmqpServerConfiguration
    {
        [ConfigurationProperty("protocol", DefaultValue = "AMQP_0_8")]
        public string Protocol
        {
            get { return this["protocol"].ToString(); }
            set { this["protocol"] = value; }
        }

        [ConfigurationProperty("address", IsRequired = true)]
        public string Address
        {
            get { return this["address"].ToString(); }
            set { this["address"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("user", DefaultValue = "guest")]
        public string User
        {
            get { return this["user"].ToString(); }
            set { this["user"] = value; }
        }

        [ConfigurationProperty("password", DefaultValue = "guest")]
        public string Password
        {
            get { return this["password"].ToString(); }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("virtualHost", DefaultValue = "/")]
        public string VirtualHost
        {
            get { return this["virtualHost"].ToString(); }
            set { this["virtualHost"] = value; }
        }
    }
}