using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Messages
{
    public class Message
    {
        public string Text { get; set; }

        public Message() { }

        public Message(string text)
        {
            Text = text;
        }
    }
}
