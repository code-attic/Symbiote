using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Connection
{
    class SelectDbCommand
        : RedisCommand<bool>
    {
        protected const string SELECT_VALUE = "*2\r\n$6\r\nSELECT\r\n${0}\r\n{1}\r\n";


        protected int DbIndex { get; set; }

        public bool Select( IConnection connection )
        {
            string idx = DbIndex.ToString();
            return connection.SendExpectSuccess( null, SELECT_VALUE.AsFormat( idx.Length, idx ) );
        }

        public SelectDbCommand(int dbIndex)
        {
            DbIndex = dbIndex;
            Command = Select;
        }

    }
}