// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System.Collections.Generic;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class KeyListCommand
        : RedisCommand<IEnumerable<string>>
    {
        protected const string KEY_LIST = "*2\r\n$4\r\nKEYS\r\n$1\r\n*\r\n";
        protected const string KEYS_LIST_MATCHING = "*2\r\n$4\r\nKEYS\r\n${0}\r\n{1}\r\n";
        protected string CommandBody { get; set; }

        public IEnumerable<string> GetList( IConnection connection )
        {
            List<byte[]> response = connection.SendExpectDataList( null, CommandBody );
            var rsltArray = new string[response.Count];
            var counter = 0;
            response.ForEach( x => rsltArray[counter++] = Encoding.UTF8.GetString( x ) );
            return rsltArray;
        }

        public KeyListCommand( string pattern )
        {
            Command = GetList;
            CommandBody = KEYS_LIST_MATCHING.AsFormat( pattern.Length, pattern );
        }

        public KeyListCommand()
        {
            CommandBody = KEY_LIST;
            Command = GetList;
        }
    }
}