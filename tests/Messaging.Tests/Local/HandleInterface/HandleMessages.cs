using System;
using System.Collections.Generic;
using Symbiote.Messaging;

namespace Messaging.Tests.Local.HandleInterface
{
    public class HandleMessages : IHandle<IAmAMessage>
    {
        public static List<string> Messages = new List<string>();

        public Action<IEnvelope> Handle( IAmAMessage message )
        {
            Messages.Add( message.Text );
            return x => x.Acknowledge();
        }
    }
}