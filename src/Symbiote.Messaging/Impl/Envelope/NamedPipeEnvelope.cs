using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using Symbiote.Core.Futures;
using Symbiote.Core.Serialization;

namespace Symbiote.Messaging.Impl.Envelope
{
    [Serializable]
    [DataContract]
    public class NamedPipeTransportEnvelope
    {
        [DataMember(Order = 1001, IsRequired = false)]
        public Guid MessageId { get; set; }

        [DataMember(Order = 1002, IsRequired = false)]
        public string MessageType { get; set; }

        [DataMember(Order = 1003, IsRequired = false)]
        public byte[] Message { get; set; }

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

        public NamedPipeTransportEnvelope()
        {
        }
    }

    [Serializable]
    [DataContract]
    public class NamedPipeEnvelope
        : IEnvelope
    {
        public Guid MessageId { get; set; }

        public Type MessageType { get; protected set; }

        public string CorrelationId { get; set; }

        public string RoutingKey { get; set; }

        public long Sequence { get; set; }

        public long Position { get; set; }

        public bool SequenceEnd { get; set; }

        public PipeStream ReplyStream { get; set; }

        public object Message { get; set; }

        public void Acknowledge()
        {
            // does not really apply ... for now 
        }

        public void Reject(string reason)
        {
            // does not really apply ... for now 
        }

        public void Reply<TResponse>(TResponse response)
        {
            if(ReplyStream != null)
            {
                var envelope = new NamedPipeTransportEnvelope()
                {
                    CorrelationId = MessageId.ToString(),
                    MessageType = typeof(TResponse).AssemblyQualifiedName,
                    Message = response.ToProtocolBuffer()
                };
                var envelopeBuffer = envelope.ToProtocolBuffer();
                ReplyStream.Write( envelopeBuffer, 0, envelopeBuffer.Length );
                ReplyStream.Flush();
            }
        }

        public NamedPipeEnvelope()
        {
        }

        public NamedPipeEnvelope(object message)
        {
            Message = message;
            MessageType = message.GetType();
            MessageId = Guid.NewGuid();
        }
    }

    [Serializable]
    [DataContract]
    public class NamedPipeEnvelope<TMessage> :
        NamedPipeEnvelope, 
        IEnvelope<TMessage>
    {
        public TMessage Message { get; set; }

        public NamedPipeServerStream ReplyStream { get; set; }

        object IEnvelope.Message
        {
            get { return this.Message; }
            set { this.Message = (TMessage) value; }
        }

        public NamedPipeEnvelope()
        {
            MessageType = typeof(TMessage);
            MessageId = Guid.NewGuid();
        }

        public NamedPipeEnvelope(TMessage message) : this()
        {
            Message = message;
        }
    }
}
