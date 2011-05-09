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

namespace Symbiote.Redis.Impl.Command.Value
{
    public class SetValueCommand<TValue>
        : RedisCommand<bool>
    {
        protected const string VALUE_EXCEEDS_1GB = "Value must not exceed 1 GB";
        protected const string SET_VALUE = "*3\r\n$3\r\nSET\r\n${0}\r\n{1}\r\n${2}\r\n";

        //protected readonly byte[] set_command_bytes = new byte[] { 42, 51, 13, 10, 36, 51, 13, 10, 83, 69, 84, 13, 10, 36 };

        protected string Key { get; set; }
        protected byte[] KeyBytes { get; set; }
        protected TValue Value { get; set; }

        public bool Set( IConnection connection )
        {
            var data = Serialize( Value );
            if ( data.Length > 1073741824 )
                throw new ArgumentException( VALUE_EXCEEDS_1GB, "value" );
            return connection.SendExpectSuccess( data, SET_VALUE.AsFormat( Key.Length, Key, data.Length ) );
        }

        //public bool Set(IConnection connection)
        //{
        //    var data = Serialize(Value);
        //    if (data.Length > 1073741824)
        //        throw new ArgumentException(VALUE_EXCEEDS_1GB, "value");
        //    return connection.SendExpectSuccess(data, GetCommandBytes(data)); //SET_VALUE.AsFormat(Key.Length, Key, data.Length));
        //}

        public SetValueCommand(string key, TValue value)
        {
            Key = key;
            //KeyBytes = Encoding.UTF8.GetBytes(Key);
            Value = value;
            Command = Set;
        }

        //protected byte[] GetCommandBytes(byte[] data)
        //{
        //    int keyLen = KeyBytes.Length;
        //    byte[] keyLenbytes = Encoding.UTF8.GetBytes(keyLen.ToString());
        //    byte[] dataLenBytes = Encoding.UTF8.GetBytes(data.Length.ToString());
        //    byte[] retArray = new byte[21 + keyLenbytes.Length + keyLen + dataLenBytes.Length]  ;
        //    set_command_bytes.CopyTo( retArray,0 );
        //    int curIdx = set_command_bytes.Length;
        //    keyLenbytes.CopyTo(retArray, curIdx);
        //    curIdx += keyLenbytes.Length;
        //    retArray[curIdx++] = 13; //\r
        //    retArray[curIdx++] = 10; //\n
        //    KeyBytes.CopyTo(retArray, curIdx);
        //    curIdx += keyLen;
        //    retArray[curIdx++] = 13; //\r
        //    retArray[curIdx++] = 10; //\n
        //    retArray[curIdx++] = 36; //$
        //    dataLenBytes.CopyTo( retArray,curIdx );
        //    curIdx += dataLenBytes.Length;
        //    retArray[curIdx++] = 13; //\r
        //    retArray[curIdx] = 10; //\n

        //    return retArray;

        //    
        //}
    }
}