using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Messages;
using Symbiote.Messaging;

namespace HelloHttp
{
    public class MessageHandler<TMessage>
        : IHandle<TMessage>
    {
        public void Handle( IEnvelope<TMessage> envelope )
        {
            envelope.Reply( new Message() { Text = "Hey there, how goes it?"} );
        }
    }
}
