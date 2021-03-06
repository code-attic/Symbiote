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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class SetManyCommand<TValue>
        : RedisCommand<bool>
    {
        protected const string SET_MANY = "*{0}\r\n$4\r\nMSET\r\n";
        protected const string RECORD = "${0}\r\n{1}\r\n${2}\r\n";
        protected static readonly byte[] NewLine = Encoding.UTF8.GetBytes( "\r\n" );

        public IEnumerable<Tuple<string, TValue>> Values { get; set; }

        public bool Set( IConnection connection )
        {
            var stream = new MemoryStream();
            Values.ForEach( v =>
                                {
                                    var val = Serialize( v.Item2 );
                                    var header = Encoding.UTF8.GetBytes(
                                        RECORD.AsFormat(
                                            v.Item1.Length,
                                            v.Item1,
                                            val.Length ) );
                                    stream.Write( header, 0, header.Length );
                                    stream.Write( val, 0, val.Length );
                                    stream.Write( NewLine, 0, NewLine.Length );
                                } );

            return connection.SendExpectSuccess( stream.ToArray(), SET_MANY.AsFormat( Values.Count()*2 + 1 ) );
        }

        public SetManyCommand( IEnumerable<Tuple<string, TValue>> values )
        {
            Values = values;
            Command = Set;
        }
    }
}