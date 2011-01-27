using System;
using System.Runtime.Serialization;
using Symbiote.Messaging;

namespace Symbiote.Http.Impl.Adapter.Messaging
{
    [Serializable]
    [DataContract]
    public class Envelope<TMessage>
        : IEnvelope<TMessage>
    {
        [IgnoreDataMember]
        public IProcessReply ProcessReply { get; set; }

        [DataMember(Order = 1001, IsRequired = false)]
        public Guid MessageId { get; set; }

        [DataMember(Order = 1002, IsRequired = false)]
        public Type MessageType { get; private set; }

        [DataMember(Order = 1003, IsRequired = false)]
        public TMessage Message { get; set; }

        [DataMember(Order = 1004, IsRequired = false)]
        public string CorrelationId { get; set; }

        [DataMember(Order = 1005, IsRequired = false)]
        public string RoutingKey { get; set; }

        [DataMember(Order = 1006, IsRequired = false)]
        public long Sequence { get; set; }

        [DataMember(Order = 1007, IsRequired = false)]
        public long Position { get; set; }

        [DataMember(Order = 1008, IsRequired = false)]
        public bool SequenceEnd { get; set; }

        public void Reply<TResponse>(TResponse response)
        {
            ProcessReply.SendReply( response );
        }

        public Envelope() { }

        public Envelope(TMessage message)
        {
            Message = message;
            MessageType = typeof(TMessage);
            MessageId = Guid.NewGuid();
        }
    }

    public interface IProcessReply
    {
        void SendReply<T>( T reply );
    }
}
