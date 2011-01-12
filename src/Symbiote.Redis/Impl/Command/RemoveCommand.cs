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
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class RemoveCommand
        : RedisCommand<bool>
    {
        private const string REMOVE = "DEL {0}\r\n";
        protected List<string> Keys { get; set; }

        public bool Remove(IConnection connection)
        {
            var count = connection.SendDataExpectInt(null, REMOVE.AsFormat(string.Join(" ", Keys)));
            return count == Keys.Count;
        }

        public RemoveCommand(string key)
        {
            Keys = new List<string>(new [] {key});
            Command = Remove;
        }

        public RemoveCommand(IEnumerable<string> keys)
        {
            Keys = new List<string>(keys);
            Command = Remove;
        }
    }
}