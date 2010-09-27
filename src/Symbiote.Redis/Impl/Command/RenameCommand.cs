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

using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class RenameCommand
        : RedisCommand<bool>
    {
        protected const string RENAME_KEY = "RENAME {0} {1}\r\n";
        public string OriginalKey { get; set; }
        public string NewKey { get; set; }

        public bool Rename(IRedisConnection connection)
        {
            return connection.SendExpectString(RENAME_KEY.AsFormat(OriginalKey, NewKey))[0] == '+';
        }

        public RenameCommand(string originalKey, string newKey)
        {
            OriginalKey = originalKey;
            NewKey = newKey;
            Command = Rename;
        }
    }
}