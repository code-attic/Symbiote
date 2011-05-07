﻿// /* 
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
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class GetCommand<TValue>
        : RedisCommand<TValue>
    {
        //protected const string GET = "GET {0}\r\n";
        protected const string GET = "*2\r\n$3\r\nGET\r\n${0}\r\n{1}\r\n";

        protected string Key { get; set; }

        public TValue Get( IConnection connection )
        {
            var data = connection.SendExpectData( null, GET.AsFormat( Key.Length, Key ) );
            return Deserialize<TValue>( data );
        }

        public GetCommand(string key)
        {
            Key = key;
            Command = Get;
        }

    }
}