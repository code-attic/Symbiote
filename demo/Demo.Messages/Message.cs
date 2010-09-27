using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Messages
{
    public class Message
    {
        public virtual string Body { get; set; }
        public virtual DateTime Created { get; set; }

        public Message()
        {
        }

        public Message(string body)
        {
            Body = body;
            Created = DateTime.Now;
        }
    }
}
