using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;
using Symbiote.Core.Serialization;

namespace Symbiote.Messaging.Impl.Serialization
{
    public class ProtobufMessageSerializer
        : IMessageSerializer
    {
        public T Deserialize<T>(byte[] message) where T : class
        {
            return message.FromProtocolBuffer<T>();
        }

        public object Deserialize(byte[] message)
        {
            throw new MessagingException(
                "Protobuf cannot deserialize messages of unknown type. You must call Deserialize<T> where T is a known message type.");
        }

        public byte[] Serialize<T>(T body) where T : class
        {
            return body.ToProtocolBuffer<T>();
        }
    }
}
