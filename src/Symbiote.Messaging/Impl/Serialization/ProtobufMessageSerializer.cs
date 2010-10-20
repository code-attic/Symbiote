using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Symbiote.Messaging.Impl.Serialization
{
    public class ProtobufMessageSerializer
        : IMessageSerializer
    {
        public T Deserialize<T>(byte[] message) where T : class
        {
            var stream = new MemoryStream(message);
            return Serializer.Deserialize<T>(stream);
        }

        public object Deserialize(byte[] message)
        {
            throw new MessagingException(
                "Protobuf cannot deserialize messages of unknown type. You must call Deserialize<T> where T is a known message type.");
        }

        public byte[] Serialize<T>(T body) where T : class
        {
            var stream = new MemoryStream();
            Serializer.Serialize<T>(stream, body);
            stream.Position = 0;
            return stream.ToArray();
        }
    }
}
