using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Jackalope;

namespace Demo.Messages
{
    public class Message
        : ICorrelate
    {
        public virtual string Body { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual string CorrelationId { get; set; }

        public Message()
        {
        }

        public Message(string body)
        {
            Body = body;
            Created = DateTime.Now;
            CorrelationId = Guid.NewGuid().ToString();
        }
    }
}
