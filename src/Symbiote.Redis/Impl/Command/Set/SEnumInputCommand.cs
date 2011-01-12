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


namespace Symbiote.Redis.Impl.Command.Set
{
    public abstract class SEnumInputCommand<TValue>
        : RedisCommand<IEnumerable<TValue>>
    {
        protected const string HEADER = "*{0}\r\n${1}\r\n{2}\r\n";
        protected const string RECORD = "${0}\r\n{1}\r\n";

        protected string CMD = string.Empty;
        protected IEnumerable<string> Key { get; set; }

        protected abstract void SetCmd();

        public IEnumerable<TValue> GetEnum(IConnection connection)
        {
            SetCmd();
            var stream = new MemoryStream();
            Key.ForEach(v =>
            {
                var header = Encoding.UTF8.GetBytes(
                    RECORD.AsFormat(
                        v.Length,
                        v));
                stream.Write(header, 0, header.Length);
            });

            var response = connection.SendExpectDataList(stream.ToArray(), HEADER.AsFormat(Key.Count() + 1, CMD.Length, CMD));
            if ((response.Count > 1) || (response[0].Length > 0))
                return response.Select(item => Deserialize<TValue>(item));
            else
                return new List<TValue>();
        }

        public SEnumInputCommand(IEnumerable<string> key)
        {
            Key = key;
            Command = GetEnum;
        }
    }
}
