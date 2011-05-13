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
using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Set
{
    internal class SAddMultiCommand<TValue>
        : RedisCommand<IEnumerable<bool>>
    {
        protected const string VALUE_EXCEEDS_1GB = "Value must not exceed 1 GB";
        protected const string SADD = "*3\r\n$4\r\nSADD\r\n${0}\r\n{1}\r\n${2}\r\n";
        protected IEnumerable<Tuple<string, TValue>> Pairs { get; set; }

        public IEnumerable<bool> SAddMulti( IConnection connection )
        {
            var sendParams = new System.Collections.Generic.List<Tuple<byte[], string>>();
            Pairs.ForEach( p =>
                               {
                                   var data = Serialize( p.Item2 );
                                   if ( data.Length > 1073741824 )
                                       throw new ArgumentException( VALUE_EXCEEDS_1GB, "value" );
                                   sendParams.Add( new Tuple<byte[], string>( data,
                                                                              SADD.AsFormat( p.Item1, data.Length ) ) );
                               }
                );
            return connection.SendExpectSuccess( sendParams );
        }

        public SAddMultiCommand( IEnumerable<Tuple<string, TValue>> pairs )
        {
            Pairs = pairs;
            Command = SAddMulti;
        }
    }
}