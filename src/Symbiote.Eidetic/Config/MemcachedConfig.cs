using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Symbiote.Eidetic.Config
{
    public class MemcachedConfig : ConfigurationSection, IMemcachedConfig
    {
        [ConfigurationProperty("servers")]
        public MemcachedServerList ServerList
        {
            get { return this["servers"] as MemcachedServerList; }
            set { this["servers"] = value; }
        }

        public IEnumerable<MemcachedServer> Servers
        {
            get
            {
                foreach(MemcachedServer server in ServerList)
                {
                    yield return server;
                }
            }
        }

        [ConfigurationProperty("minpoolsize", DefaultValue = 10)]
        public int MinPoolSize
        {
            get { return (int)this["minpoolsize"]; }
            set { this["minpoolsize"] = value; }
        }

        [ConfigurationProperty("maxpoolsize", DefaultValue = 100)]
        public int MaxPoolSize
        {
            get { return (int)this["maxpoolsize"]; }
            set { this["maxpoolsize"] = value; }
        }

        [ConfigurationProperty("timeout", DefaultValue = 10)]
        public int Timeout
        {
            get { return (int)this["timeout"]; }
            set { this["timeout"] = value; }
        }

        [ConfigurationProperty("deadtimeout", DefaultValue = 30)]
        public int DeadTimeout
        {
            get { return (int)this["deadtimeout"]; }
            set { this["deadtimeout"] = value; }
        }
    }
}
