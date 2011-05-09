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
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.List
{
    internal class LIndexCommand<TValue>
        : RedisCommand<TValue>
    {
        protected const string LINDEX = "*3\r\n$6\r\nLINDEX\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n";
        protected string Key { get; set; }
        protected int Index { get; set; }
        protected TValue Value { get; set; }

        public TValue LIndex( IConnection connection )
        {
            var indexLen = Index / 10 + 1 + (Index < 0 ? 1 : 0);
            var data = connection.SendExpectData( null, LINDEX.AsFormat( Key.Length, Key, indexLen, Index ) );
            return Deserialize<TValue>( data );
        }

        public LIndexCommand( string key, int index )
        {
            Key = key;
            Index = index;
            Command = LIndex;
        }
    }
}