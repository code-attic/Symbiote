using System;
using System.Linq;
using System.Text;
using Symbiote.Relax.Impl;

namespace Symbiote.Relax
{
    public class CouchConfigurator
    {
        private CouchConfiguration _config = new CouchConfiguration();

        public CouchConfigurator Cache()
        {
            _config.Cache = true;
            _config.CacheExpiration = DateTime.MaxValue;
            return this;
        }

        public CouchConfigurator Cache(DateTime expiration)
        {
            _config.Cache = true;
            _config.CacheExpiration = expiration;
            return this;
        }

        public CouchConfigurator Cache(TimeSpan timeLimit)
        {
            _config.Cache = true;
            _config.CacheLimit = timeLimit;
            return this;
        }

        public CouchConfigurator Https()
        {
            _config.Protocol = "https";
            return this;
        }

        public CouchConfigurator UseForType<T>(string datbaseName)
        {
            _config.SetDatabaseNameForType<T>(datbaseName);
            return this;
        }

        public CouchConfigurator Port(int port)
        {
            _config.Port = port;
            return this;
        }

        public CouchConfigurator Preauthorize()
        {
            _config.Preauthorize = true;
            return this;
        }

        public CouchConfigurator TimeOut(int timeOut)
        {
            _config.TimeOut = timeOut;
            return this;
        }

        public CouchConfigurator Server(string server)
        {
            _config.Server = server;
            return this;
        }

        public ICouchConfiguration GetConfiguration()
        {
            return _config;
        }
    }
}
