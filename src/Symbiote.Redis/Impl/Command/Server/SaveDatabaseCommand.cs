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
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command.Server
{
    public class SaveDatabaseCommand
        : RedisCommand<bool>
    {
        protected const string SAVE = "*1\r\n$4\r\nSAVE\r\n";
        protected const string SAVE_ASYNC = "*1\r\n$6\r\nBGSAVE\r\n";
        public bool Synchronous { get; set; }
        protected string CommandStr;

        public bool Save( IConnection connection )
        {
            var rslt = connection.SendExpectString(Synchronous ? SAVE : SAVE_ASYNC);
            return rslt == (Synchronous ? "OK" : "Background saving started");
        }

        public SaveDatabaseCommand(bool synchronously)
        {
            Synchronous = synchronously;

            Command = Save;
        }

    }
}