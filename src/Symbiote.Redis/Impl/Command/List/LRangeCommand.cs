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
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.List
{
    public class LRangeCommand<T>
        : RedisCommand<IEnumerable<T>>
    {
        protected const string LRANGE = "*4\r\n$6\r\nLRANGE\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n";
        protected string Key { get; set; }
        protected int StartIndex { get; set; }
        protected int EndIndex { get; set; }

        public IEnumerable<T> LRange( IConnection connection )
        {
            var startLength = StartIndex / 10 + 1 + (StartIndex < 0 ? 1 : 0);
            var endLength = EndIndex / 10 + 1 + (EndIndex < 0 ? 1 : 0);
            var response = connection.SendExpectDataList( null, LRANGE.AsFormat( Key.Length, Key, startLength, StartIndex, endLength, EndIndex ) );
            return response.Select( item => Deserialize<T>( item ) ); //.ToList();
        }

        public LRangeCommand( string key, int startIndex, int endIndex )
        {
            Key = key;
            StartIndex = startIndex;
            EndIndex = endIndex;
            Command = LRange;
        }
    }
}