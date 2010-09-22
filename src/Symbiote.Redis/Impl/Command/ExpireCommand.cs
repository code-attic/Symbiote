using System;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class ExpireCommand
        : RedisCommand<bool>
    {
        protected const string EXPIRE = "EXPIRE {0} {1}\r\n";
        protected const string EXPIRE_AT = "EXPIREAT {0} {1}\r\n";
        protected DateTime ExpireOn { get; set; }
        protected int ExpireIn { get; set; }
        protected string CommandBody { get; set; }
        public string Key { get; set; }
        
        public bool SetExpiration(IRedisConnection connection)
        {
            return connection.SendDataExpectInt(null, CommandBody) == 1;
        }

        public ExpireCommand(string key, DateTime expireOn)
        {
            Key = key;
            ExpireOn = expireOn;
            CommandBody = EXPIRE_AT.AsFormat(key, expireOn.ToFileTime());
        }

        public ExpireCommand(string key, int expireIn)
        {
            Key = key;
            ExpireIn = expireIn;
            CommandBody = EXPIRE.AsFormat(key, expireIn);
        }
    }
}