using System.Collections.Generic;
using System.Text;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class InfoCommand
        : RedisCommand<Dictionary<string, string>>
    {
        protected const string INFO = "INFO\r\n";

        public Dictionary<string,string> GetInfo(IRedisConnection connection)
        {
            byte[] r = connection.SendExpectData(null, INFO);
            var dict = new Dictionary<string, string>();

            foreach (var line in Encoding.UTF8.GetString(r).Split('\n'))
            {
                int p = line.IndexOf(':');
                if (p == -1)
                    continue;
                dict.Add(line.Substring(0, p), line.Substring(p + 1));
            }
            return dict;
        }

        public InfoCommand()
        {
            Command = GetInfo;
        }
    }
}