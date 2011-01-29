using System;
using System.IO;
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

        public static object FromProtocolBuffer(this Stream stream, Type type)
        {
            stream.Position = 0;
            return Serializer.NonGeneric.Deserialize( type, stream );
        }

        public static object FromProtocolBuffer(this byte[] bytes, Type type)
        {
            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                return Serializer.NonGeneric.Deserialize(type, stream);
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
