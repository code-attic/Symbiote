using System;
using Minion.Messages;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Messaging;

namespace Minion.Hosted
{
    public class CommandHandler : IHandle<MinionDoThis>
    {
        public Action<IEnvelope> Handle( MinionDoThis message )
        {
            "Received: {0}".ToInfo<IMinion>( message.Text );
            return x => x.Acknowledge();
        }
    }
}