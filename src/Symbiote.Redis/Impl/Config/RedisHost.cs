namespace Symbiote.Redis.Impl.Config
{
    public class RedisHost
    {
        public string Host { get; private set; }
        public int Port { get; private set; }

        public void Init()
        {
            Host = "localhost";
            Port = 6379;
        }

        public RedisHost()
        {
            Init();
        }

        public RedisHost(string host, int port)
        {
            Host = host;
            Port = port;
        }
    }
}