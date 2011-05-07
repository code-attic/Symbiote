using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class ExistsCommand  : RedisCommand<bool>
    {
        protected const string EXISTS = "*2\r\n$6\r\nEXISTS\r\n${0}\r\n{1}\r\n";
        protected string Key { get; set; }

        public bool Exists( IConnection connection )
        {
            return connection.SendDataExpectInt( null, EXISTS.AsFormat( Key.Length, Key ) ) == 1;
        }

        public ExistsCommand( string key )
        {
            Key = key;
            Command = Exists;
        }
    }
}