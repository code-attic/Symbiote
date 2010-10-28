using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Messaging.Impl.Envelope
{
    public class Envelope<TMessage>
        : IEnvelope<TMessage>
    {
        public string CorrelationId { get; set; }
        public string RoutingKey { get; set; }
        public long Sequence { get; set; }
        public long Position { get; set; }
        public bool SequenceEnd { get; set; }
        public Guid MessageId { get; set; }
        public Type MessageType { get; private set; }
        public TMessage Message { get; set; }

        public Envelope( TMessage message )
        {
            Message = message;
            MessageType = typeof(TMessage);
            MessageId = Guid.NewGuid();
        }
    }
}
