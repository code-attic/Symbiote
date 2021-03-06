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
using System.Runtime.Serialization.Formatters.Binary;

namespace Symbiote.Messaging.Impl.Serialization
{
    public class NetBinarySerializer : IMessageSerializer
    {
        public T Deserialize<T>( byte[] message )
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream( message );
            var body = (T) formatter.Deserialize( stream );
            return body;
        }

        public object Deserialize( byte[] message )
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream( message );
            var body = formatter.Deserialize( stream );
            return body;
        }

        public object Deserialize( Type messageType, byte[] message )
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream( message );
            var body = formatter.Deserialize( stream );
            return body;
        }

        public byte[] Serialize<T>( T body )
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize( stream, body );
            return stream.ToArray();
        }
    }
}