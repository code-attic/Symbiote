using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Symbiote.Core.Serialization
{
    public static class ProtobufExtensions
    {
        public static T FromProtocolBuffer<T>(this Stream stream)
        {
            stream.Position = 0;
            return Serializer.Deserialize<T>( stream );
        }

        public static T FromProtocolBuffer<T>(this byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                return Serializer.Deserialize<T>( stream );
            }
        }

        public static byte[] ToProtocolBuffer<T>(this T instance)
        {
            using(var stream = new MemoryStream())
            {
                Serializer.Serialize( stream, instance );
                return stream.ToArray();
            }
        }

        public static void ToProtocolBuffer<T>(this T instance, Stream stream)
        {
            Serializer.Serialize( stream, instance );
        }
    }
}
