/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;

namespace Symbiote.Messaging.Impl.Serialization
{
    public class MessageSerializer
    {
        public static Type GetBestMessageSerializerFor<T>()
        {
            var type = typeof(T);
            return GetBestMessageSerializer( type );
        }

        public static Type GetBestMessageSerializer( Type type )
        {
            Type serializerType;
            if(type.IsProtobufSerializable())
            {
                serializerType = typeof(ProtobufMessageSerializer);
            }
            else if(type.IsBinarySerializable())
            {
                serializerType = typeof(NetBinarySerializer);
            }
            else if(type.IsJsonSerializable())
            {
                serializerType = typeof(JsonMessageSerializer);
            }
            else
            {
                throw new MessagingException(
                    "The type {0} cannot be processed by any serializers. Check that you have a default constructor and that all message properties are compatible with the desired message serializer."
                        .AsFormat( type.FullName )
                    );
            }
            return serializerType;
        }
    }
}
