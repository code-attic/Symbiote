using System.Collections.Concurrent;

namespace Symbiote.Redis.Impl.Config
{
    public class RedisConfiguration
    {
        public ConcurrentDictionary<string, RedisHost> Hosts { get; set; }
        public int RetryTimeout { get; set; }
        public int RetryCount { get; set; }
        public int SendTimeout { get; set; }
        public string Password { get; set; }
        public int ConnectionLimit { get; set; }

        public void Init()
        {
            ConnectionLimit = 10;
            SendTimeout = 30;
            RetryTimeout = 30;
            RetryCount = 5;
        }

        public RedisConfiguration()
        {
            Hosts = new ConcurrentDictionary<string, RedisHost>();
            Init();
        }
    }
}