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

namespace Symbiote.Redis.Impl.Command.Hash
{
    public class HGetAllCommand<T>
        : RedisCommand<IDictionary<string, T>>
    {
        protected const string HVALS = "*2\r\n$7\r\nHGETALL\r\n${0}\r\n{1}\r\n";
        protected string Key { get; set; }

        public IDictionary<string, T> HGetAll<T>( IConnection connection )
        {
            var response = connection.SendExpectDataDictionary( null, HVALS.AsFormat( Key.Length, Key ) );
            return response.ToDictionary( x => x.Key, x => Deserialize<T>( x.Value ) );
        }

        public HGetAllCommand( string key )
        {
            Key = key;
            Command = HGetAll<T>;
        }
    }
}