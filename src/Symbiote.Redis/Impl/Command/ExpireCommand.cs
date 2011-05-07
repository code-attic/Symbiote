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

namespace Symbiote.Redis.Impl.Command
{
    public class ExpireCommand
        : RedisCommand<bool>
    {
        protected const string EXPIRE = "*3\r\n$6\r\nEXPIRE\r\n${0}\r\n{1}\r\n$2\r\n{3}\r\n";
        protected const string EXPIRE_AT = "*3\r\n$8\r\nEXPIREAT\r\n${0}\r\n{1}\r\n$2\r\n{3}\r\n";
        protected DateTime ExpireOn { get; set; }
        protected int ExpireIn { get; set; }
        protected string CommandBody { get; set; }
        public string Key { get; set; }

        public bool SetExpiration( IConnection connection )
        {
            return connection.SendDataExpectInt( null, CommandBody ) == 1;
        }

        public ExpireCommand( string key, DateTime expireOn )
        {
            Key = key;
            ExpireOn = expireOn;
            long fileTime = expireOn.ToFileTime();
            CommandBody = EXPIRE_AT.AsFormat( key.Length,key, fileTime/10+1,  fileTime );
        }

        public ExpireCommand( string key, int expireIn )
        {
            Key = key;
            ExpireIn = expireIn;
            CommandBody = EXPIRE.AsFormat(key.Length, key, expireIn / 10 + 1, expireIn);
        }
    }
}