using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Messaging;

namespace RabbitDemo.Messages
{
    public class Message
        : ICorrelate
    {
        public string CorrelationId { get; set; }
        public int MessageId { get; set; }

        public Message(string correlationId, int id)
        {
            CorrelationId = correlationId;
            MessageId = id;
        }
    }
}
