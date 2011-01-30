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

namespace Symbiote.Redis.Impl.Command.Hash
{
    internal class HGetCommand<TValue>
        : RedisCommand<TValue>
    {
        protected const string HGET = "HGET {0} {1}\r\n";
        protected string Key { get; set; }
        protected string Field { get; set; }

        public TValue HGet( IConnection connection )
        {
            //For some reason HGet isn't working properly.  Workaround is to use HMGet until I have time to sort it out
            //var data = connection.SendExpectData(null, HGET.AsFormat(Key, Field));
            //return Deserialize<TValue>(data);
            var fields = new System.Collections.Generic.List<string>();
            fields.Add( Field );
            var manyCmd = new HGetManyCommand<TValue>( Key, fields );
            var mreturns = manyCmd.HGet( connection );
            TValue rslt = default(TValue);
            mreturns.ForEach( v => rslt = v );
            return rslt;
        }

        public HGetCommand( string key, string field )
        {
            Key = key;
            Field = field;
            Command = HGet;
        }
    }
}