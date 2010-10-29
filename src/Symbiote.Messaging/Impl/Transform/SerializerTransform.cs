using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging.Impl.Serialization;

namespace Symbiote.Messaging.Impl.Transform
{
    public class SerializerTransform<TMessage>
        : BaseTransform<TMessage, byte[]>
    {
        protected IMessageSerializer SerializationProvider { get; set; }

        public override byte[] Transform( TMessage origin )
        {
            return SerializationProvider.Serialize( origin );
        }

        public override TMessage Reverse( byte[] transformed )
        {
            return SerializationProvider.Deserialize<TMessage>( transformed );
        }

        public SerializerTransform()
        {
            var serializerType = MessageSerializer.GetBestMessageSerializerFor<TMessage>();
            SerializationProvider = Activator.CreateInstance( serializerType ) as IMessageSerializer;
        }
    }
}
