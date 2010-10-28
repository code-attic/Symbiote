using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;

namespace Symbiote.Messaging.Impl.Serialization
{
    public class MessageSerializer
    {
        public static Type GetBestMessageSerializerFor<T>()
        {
            var type = typeof(T);
            if(type.IsProtobufSerializable())
            {
                return typeof(ProtobufMessageSerializer);
            }
            else if(type.IsBinarySerializable())
            {
                return typeof(NetBinarySerializer);
            }
            else if(type.IsJsonSerializable())
            {
                return typeof(JsonMessageSerializer);
            }
            else
            {
                throw new MessagingException(
                    "The type {0} cannot be processed by any serializers. Check that you have a default constructor and that all message properties are compatible with the desired message serializer."
                        .AsFormat( type.FullName )
                    );
            }
        }
    }
}
