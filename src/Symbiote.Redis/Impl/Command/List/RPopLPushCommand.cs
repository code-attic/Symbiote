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
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.List
{
    internal class RPopLPushCommand
        : RedisCommand<bool>
    {
        protected const string RPLP = "*3\r\n$9\r\nRPOPLPUSH\r\n${0}\r\n{1}\r\n${2}\r\n{3}\r\n";
        protected string SrcKey { get; set; }
        protected string DestKey { get; set; }

        public bool RPopLPush( IConnection connection )
        {
            return connection.SendExpectSuccess( null, RPLP.AsFormat( SrcKey.Length, SrcKey, DestKey.Length, DestKey ) );
        }

        public RPopLPushCommand( string srcKey, string destKey )
        {
            SrcKey = srcKey;
            DestKey = destKey;
            Command = RPopLPush;
        }
    }
}