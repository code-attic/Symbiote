using System;
using System.Linq;
using System.Threading;
using Minion.Messages;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Daemon.BootStrap;
using Symbiote.Messaging;

namespace Minion.Host
{
    public class NotificationHandler : IHandle<MinionUp>
    {
        public IBus Bus { get; set; }

        public Action<IEnvelope> Handle( MinionUp message )
        {
            "Minion says: {0}"
                .ToInfo<IMinion>( message.Text );

            Enumerable.Range(0, 10).ForEach( x => 
                Bus.Publish( "Hosted", new MinionDoThis() { Text = "Command {0}".AsFormat( x )} ) );

            Thread.Sleep( 1000 );

            Bus.Publish( "Hosted", new HaltMinion() );

            return x => x.Acknowledge();
        }

        public NotificationHandler( IBus bus )
        {
            Bus = bus;
        }
    }
}