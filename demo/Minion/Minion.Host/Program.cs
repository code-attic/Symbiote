using System;
using System.Linq;
using Minion.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Messaging;
using Symbiote.Log4Net;
using Symbiote.Rabbit;

namespace Minion.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .AddConsoleLogger<IDaemon>( l => l.Info().MessageLayout( m => m.Message().Newline() ) )
                .Daemon( x => x.Arguments( args ).WithBootStraps( b => b.HostApplicationsFrom( @"C:\git\Symbiote\demo\Minion\Minions" ) ) )
                .Rabbit( x => x.AddBroker( r => r.Defaults() ) )
                .RunDaemon();
        }

        public class Host : IDaemon
        {
            public IBus Bus { get; set; }

            public void Start()
            {
                "Host has started."
                    .ToInfo<IDaemon>();
                Bus.AddLocalChannel();
                Bus.AddRabbitChannel(x => x.Direct("Host").AutoDelete());
                Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Host").ExchangeName("Host").StartSubscription().NoAck());

                Bus.AddRabbitChannel(x => x.Direct("Hosted").AutoDelete());
                Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Hosted").ExchangeName("Hosted").NoAck());
            }

            public void Stop()
            {
                
            }

            public Host( IBus bus )
            {
                Bus = bus;
            }
        }

        public class NotificationHandler : IHandle<MinionUp>
        {
            public IBus Bus { get; set; }

            public Action<IEnvelope> Handle( MinionUp message )
            {
                "Minion says: {0}"
                    .ToInfo<IDaemon>( message.Text );

                Enumerable
                    .Range(0, 10)
                    .ForEach( x => Bus.Publish( "Hosted", new MinionDoThis() { Text = "Command {0}".AsFormat( x )} ) );
                return x => x.Acknowledge();
            }

            public NotificationHandler( IBus bus )
            {
                Bus = bus;
            }
        }
    }
}
