using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.SocketMQ;

namespace SocketMQ.Tests
{
    [Serializable]
    public class ElaborateMessageType
    {
        public virtual string Content { get; set; }
        public virtual bool Valid { get; set; }
        public virtual string Sender { get; set; }
    }
}
