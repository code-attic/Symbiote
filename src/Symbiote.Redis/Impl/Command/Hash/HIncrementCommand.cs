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

namespace Symbiote.Redis.Impl.Command
{
    public class HIncrementCommand
        : RedisCommand<int>
    {
        protected const string INCREMENT_BY = "*4\r\n$7\r\nHINCRBY\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n{5}\r\n";
        protected int IncrementBy { get; set; }
        protected string Key { get; set; }
        protected string Field { get; set; }

        public int Increment( IConnection connection )
        {
            var stepWidth = IncrementBy / 10 + 1 + ((IncrementBy < 0) ? 1 : 0);
            return connection.SendDataExpectInt( null, INCREMENT_BY.AsFormat( Key.Length, Key, Field.Length, Field, stepWidth, IncrementBy ) );
        }

        public HIncrementCommand( int incrementBy, string key, string field )
        {
            IncrementBy = incrementBy;
            Key = key;
            Field = field;
            Command = Increment;
        }
    }
}