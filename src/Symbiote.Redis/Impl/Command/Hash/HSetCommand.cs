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
using System;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Hash
{
    internal class HSetCommand<TValue>
        : RedisCommand<bool>
    {
        protected const string VALUE_EXCEEDS_1GB = "Value must not exceed 1 GB";
        protected const string HSET = "*4\r\n$4\r\nHSET\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n${4}\r\n";
        protected string Key { get; set; }
        protected string Field { get; set; }
        protected TValue Value { get; set; }

        public bool HSet( IConnection connection )
        {
            var data = Serialize( Value );
            if ( data.Length > 1073741824 )
                throw new ArgumentException( VALUE_EXCEEDS_1GB, "value" );
            return connection.SendDataExpectInt(data, HSET.AsFormat(Key.Length, Key, Field.Length, Field, data.Length)) == 1;
        }

        public HSetCommand( string key, string field, TValue value )
        {
            Key = key;
            Field = field;
            Value = value;
            Command = HSet;
        }
    }
}