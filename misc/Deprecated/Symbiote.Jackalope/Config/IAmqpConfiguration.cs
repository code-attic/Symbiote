using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Symbiote.Jackalope.Config
{
    public class AmqpConfiguration : ConfigurationSection, IAmqpConfigurationProvider
    {
        public IList<IAmqpServerConfiguration> Servers
        {
            get
            {
                var list = new List<IAmqpServerConfiguration>();
                foreach (AmqpServerConfiguration server in ServerList)
                {
                    list.Add(server);
                }
                return list;
            }
            set { /* do nothing */ }
        }

        [ConfigurationCollection(typeof(AmqpServerCollection), AddItemName = "add")]
        [ConfigurationProperty("servers")]
        public AmqpServerCollection ServerList
        {
            get { return this["servers"] as AmqpServerCollection; }
            set { this["servers"] = value; }
        }
    }
}
