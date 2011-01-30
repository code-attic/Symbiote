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
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public static class ByteExtensions
    {
        private static readonly int BUFFER_LENGTH = 8*1024;

        public static string FromBytes( this byte[] bytes )
        {
            return bytes == null || bytes.Length == 0 ? null : Encoding.UTF8.GetString( bytes );
        }

        public static byte[] ToBytes( this string value )
        {
            return value == null ? null : Encoding.UTF8.GetBytes( value );
        }

        public static byte[] ToBytes( this NetworkStream stream )
        {
            var buffer = new byte[BUFFER_LENGTH];
            using( var memoryStream = new MemoryStream() )
            {
                do
                {
                    try
                    {
                        var read = stream.Read( buffer, 0, buffer.Length );
                        memoryStream.Write( buffer, 0, read );
                    }
                    catch ( Exception e )
                    {
                        //do nothing for now
                    }
                } while ( stream.DataAvailable );
                return memoryStream.ToArray();
            }
        }
    }
}