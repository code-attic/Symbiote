using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;


namespace Symbiote.Redis.Impl.Serialization
{
    class ProtobufCacheSerializer: ICacheSerializer
    {

        public byte[] Serialize<T>(T value)
        {
            MemoryStream stream = new MemoryStream();

            Serializer.Serialize(stream, value);
            //Serializer.SerializeWithLengthPrefix(stream, value, PrefixStyle.Base128);
            return stream.ToArray();
        }

        public T Deserialize<T>(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            return Serializer.Deserialize<T>(stream);
            //return Serializer.DeserializeWithLengthPrefix<T>(stream, PrefixStyle.Base128);
        }
    }
}
