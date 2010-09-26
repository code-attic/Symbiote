using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class SetManyCommand<TValue>
        : RedisCommand<bool>
    {
        protected const string SET_MANY = "*{0}\r\n$4\r\nMSET\r\n";
        protected const string RECORD = "${0}\r\n{1}\r\n${2}\r\n";
        protected static readonly byte[] NewLine = UTF8Encoding.UTF8.GetBytes("\r\n");

        public IEnumerable<Tuple<string, TValue>> Values { get; set; }

        public bool Set(IRedisConnection connection)
        {
            var stream = new MemoryStream();
            Values.ForEach(v =>
                               {
                                   var val = Serialize(v.Item2);
                                   var header = Encoding.UTF8.GetBytes(
                                       RECORD.AsFormat(
                                           v.Item1.Length,
                                           v.Item1,
                                           val.Length));
                                   stream.Write(header, 0, header.Length);
                                   stream.Write(val, 0, val.Length);
                                   stream.Write(NewLine, 0, NewLine.Length);
                               });

            return connection.SendExpectSuccess(stream.ToArray(), SET_MANY.AsFormat(Values.Count()*2 + 1));
        }

        public SetManyCommand(IEnumerable<Tuple<string, TValue>> values)
        {
            Values = values;
            Command = Set;
        }
    }
}
