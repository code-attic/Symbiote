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
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Server
{
    public class LastSaveCommand
        : RedisCommand<DateTime>
    {
        protected const long EPOCH = 621355968000000000L;
        protected const string LAST_SAVE = "*1\r\n$8\r\nLASTSAVE\r\n";

        public DateTime GetLast( IConnection connection )
        {
            var last = connection.SendDataExpectInt( null, LAST_SAVE );
            return new DateTime( EPOCH ).AddSeconds( last );
        }

        public LastSaveCommand()
        {
            Command = GetLast;
        }
    }
}