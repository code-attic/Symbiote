using System;
using System.Collections.Concurrent;
using Symbiote.Messaging.Impl.Serialization;
using Symbiote.Messaging.Impl.Transform;

namespace Symbiote.Rabbit.Impl.Transform
{
    public class RabbitSerializerTransform
        : BaseTransform<RabbitEnvelope, RabbitEnvelope>
    {
        protected ConcurrentDictionary<Type, IMessageSerializer> SerializationProviders { get; set; }

        public IMessageSerializer GetSerialzier(Type messageType)
        {
            IMessageSerializer serializer = null;
            if (!SerializationProviders.TryGetValue(messageType, out serializer))
            {
                var serializerType = MessageSerializer.GetBestMessageSerializer(messageType);
                serializer = Activator.CreateInstance(serializerType) as IMessageSerializer;
                SerializationProviders.TryAdd(messageType, serializer);
            }
            return serializer;
        }

        public override RabbitEnvelope Transform(RabbitEnvelope origin)
        {
            origin.ByteStream = GetSerialzier(origin.MessageType).Serialize(origin.Message);
            return origin;
        }

        public override RabbitEnvelope Reverse(RabbitEnvelope origin)
        {
            origin.Message = GetSerialzier(origin.MessageType).Deserialize(origin.MessageType, origin.ByteStream);
            return origin;
        }

        public RabbitSerializerTransform()
        {
            SerializationProviders = new ConcurrentDictionary<Type, IMessageSerializer>();
        }
    }
}
