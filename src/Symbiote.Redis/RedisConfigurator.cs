using Symbiote.Redis.Impl;
using Symbiote.Redis.Impl.Config;

namespace Symbiote.Redis
{
    public class RedisConfigurator
    {
        public RedisConfiguration Configuration { get; set; }

        public RedisConfigurator AddServer(string host, int port)
        {
            var server = new RedisHost(host, port);
            Configuration.Hosts[host] = server;
            return this;
        }

        public RedisConfigurator AddServer(string host)
        {
            var server = new RedisHost() { Host = host };
            Configuration.Hosts[host] = server;
            return this;
        }

        public RedisConfigurator AddLocalServer()
        {
            var server = new RedisHost();
            Configuration.Hosts["local"] = server;
            return this;
        }

        public RedisConfigurator LimitPoolConnections(int limit)
        {
            Configuration.ConnectionLimit = limit;
            return this;
        }

        public RedisConfigurator Password(string password)
        {
            Configuration.Password = password;
            return this;
        }

        public RedisConfigurator Retries(int retries)
        {
            Configuration.RetryCount = retries;
            return this;
        }

        public RedisConfigurator RetryTimeout(int miliseconds)
        {
            Configuration.RetryTimeout = miliseconds;
            return this;
        }

        public RedisConfigurator SendTimeout(int miliseconds)
        {
            Configuration.SendTimeout = miliseconds;
            return this;
        }

        public RedisConfigurator()
        {
            Configuration = new RedisConfiguration();
        }
    }
}