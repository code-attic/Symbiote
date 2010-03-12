using System;
using System.Collections.Generic;

namespace Symbiote.Relax.Impl
{
    public class CouchConfiguration : ICouchConfiguration
    {
        protected Dictionary<Type, string> _databaseForType = new Dictionary<Type, string>();

        public string GetDatabaseNameForType<T>()
        {
            var type = typeof (T);
            var dbname = type.Name;
            _databaseForType.TryGetValue(type, out dbname);
            return dbname;
        }

        public void SetDatabaseNameForType<T>(string databaseName)
        {
            _databaseForType[typeof (T)] = databaseName;
        }

        public string Protocol { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool Preauthorize { get; set; }
        public int TimeOut { get; set; }
        public bool Cache { get; set; }
        public DateTime CacheExpiration { get; set; }
        public TimeSpan CacheLimit { get; set; }

        public CouchConfiguration()
        {
            Protocol = "http";
            Server = "localhost";
            Port = 5984;
            Preauthorize = false;
            TimeOut = 6000;
        }
    }
}