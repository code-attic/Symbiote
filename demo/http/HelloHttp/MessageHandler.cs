using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Messages;
using Symbiote.Messaging;

namespace HelloHttp
{
    public class MessageHandler
        : IHandle<Message>
    {
        public Action<IEnvelope> Handle(Message envelope)
        {
            return x => x.Reply( new Message() { Text = "Hey there, how goes it?"} );
        }
    }
}
