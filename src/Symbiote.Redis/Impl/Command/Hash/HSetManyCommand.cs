/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Hash
{
    public class HSetManyCommand<TValue>
        : RedisCommand<bool>
    {
        protected const string HSET_MANY = "*{0}\r\n$5\r\nHMSET\r\n${1}\r\n{2}\r\n";
        protected const string RECORD = "${0}\r\n{1}\r\n${2}\r\n";
        protected static readonly byte[] NewLine = UTF8Encoding.UTF8.GetBytes("\r\n");

        protected string Key { get; set; }
        protected IEnumerable<Tuple<string, TValue>> Values { get; set; }

        public bool HSet(IRedisConnection connection)
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

            return connection.SendExpectSuccess(stream.ToArray(), HSET_MANY.AsFormat(Values.Count() * 2 + 2, Key.Length, Key));
        }

        public HSetManyCommand(string key, IEnumerable<Tuple<string, TValue>> values)
        {
            Key = key;
            Values = values;
            Command = HSet;
        }
    }
}
