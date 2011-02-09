using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Symbiote.Core.Futures;
using Symbiote.Core.Serialization;
using Symbiote.Messaging.Impl.Channels.Pipe;

namespace Symbiote.Messaging.Impl.Envelope
{
    [Serializable]
    [DataContract]
    public class NamedPipeEnvelope
        : IEnvelope
    {
        [IgnoreDataMember]
        [JsonIgnore]
        [NonSerialized]
        private PipeProxy _proxy;

        [DataMember(Order = 1001, IsRequired = false)]
        public Guid MessageId { get; set; }

        [DataMember(Order = 1002, IsRequired = false)]
        public Type MessageType { get; set; }

        [DataMember(Order = 1003, IsRequired = false)]
        public object Message { get; set; }

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

        [IgnoreDataMember]
        [JsonIgnore]
        public PipeProxy ReplyStream
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

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
            if (ReplyStream != null)
            {
                var envelope = new NamedPipeEnvelope()
                {
                    CorrelationId = MessageId.ToString(),
                    MessageType = typeof(TResponse),
                    Message = response
                };
                ReplyStream.Send( envelope );
            }
        }

        public NamedPipeEnvelope()
        {
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
