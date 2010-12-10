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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Hash
{
    public class HKeysCommand
        : RedisCommand<IEnumerable<string>>
    {
        protected string Key { get; set; }
        protected const string HKEYS = "HKEYS {0}\r\n";

        public IEnumerable<string> HKeys(IRedisConnection connection)
        {
            var response = connection.SendExpectDataList(null, HKEYS.AsFormat(Key));
            return response.Select(item => Encoding.UTF8.GetString(item)); //.ToList();
        }

        public HKeysCommand(string key)
        {
            Key = key;
            Command = HKeys;
        }

    }
}