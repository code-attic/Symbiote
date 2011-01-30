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
using ProtoBuf;

namespace Symbiote.Core.Serialization
{
    public static class ProtobufExtensions
    {
        public static T FromProtocolBuffer<T>( this Stream stream )
        {
            stream.Position = 0;
            return Serializer.Deserialize<T>( stream );
        }

        public static T FromProtocolBuffer<T>( this byte[] bytes )
        {
            using( var stream = new MemoryStream( bytes ) )
            {
                stream.Position = 0;
                return Serializer.Deserialize<T>( stream );
            }
        }

        public static object FromProtocolBuffer( this Stream stream, Type type )
        {
            stream.Position = 0;
            return Serializer.NonGeneric.Deserialize( type, stream );
        }

        public static object FromProtocolBuffer( this byte[] bytes, Type type )
        {
            using( var stream = new MemoryStream( bytes ) )
            {
                stream.Position = 0;
                return Serializer.NonGeneric.Deserialize( type, stream );
            }
        }

        public static byte[] ToProtocolBuffer<T>( this T instance )
        {
            using( var stream = new MemoryStream() )
            {
                Serializer.Serialize( stream, instance );
                return stream.ToArray();
            }
        }

        public static void ToProtocolBuffer<T>( this T instance, Stream stream )
        {
            Serializer.Serialize( stream, instance );
        }
    }
}