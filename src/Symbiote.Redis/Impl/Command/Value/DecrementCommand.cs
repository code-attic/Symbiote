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

namespace Symbiote.Redis.Impl.Command.Value
{
    public class DecrementCommand
        : RedisCommand<int>
    {
        protected const string DECREMENT = "*2\r\n$4\r\nDECR\r\n${0}\r\n{1}\r\n";
        protected const string DECREMENT_BY = "*3\r\n$6\r\nDECRBY\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n";
        protected int IncrementBy { get; set; }
        protected string Key { get; set; }

        public int Increment( IConnection connection )
        {
            
            var command =
                IncrementBy > 1
                    ? DECREMENT_BY.AsFormat( Key.Length, Key, IncrementBy/10 +1, IncrementBy )
                    : DECREMENT.AsFormat( Key.Length, Key );

            return connection.SendDataExpectInt( null, command );
        }

        public DecrementCommand( int incrementBy, string key )
        {
            IncrementBy = incrementBy;
            Key = key;
            Command = Increment;
        }
    }
}