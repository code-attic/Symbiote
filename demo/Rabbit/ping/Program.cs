using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using pingpong.messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Messaging;
using Symbiote.Rabbit;
using Symbiote.Log4Net;

namespace ping
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .Rabbit( x => x.AddBroker( b => b.Defaults() ) )
                .AddConsoleLogger<PingService>( x => x.Debug().MessageLayout( m => m.Message().Newline() ) )
                .Daemon( x => x.Name( "ping" ) )
                .RunDaemon();
        }
    }

    public class PingService : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Bus.AddRabbitChannel( x => x.Direct( "ping" ) );
            Bus.AddRabbitQueue( x => x.ExchangeName( "ping" ).QueueName( "ping" ).NoAck() );


            Enumerable.Range( 0, 100 ).ForEach( x => 
            {
                Bus.Request<Ping, Pong>( "ping", new Ping() ).OnValue( p => 
                    "Received pong!".ToInfo<PingService>()
                    );
                Thread.Sleep( 1000 );
            } );
        }

        public void Stop()
        {
            
        }

        public PingService( IBus bus )
        {
            Bus = bus;
        }
    }

    public class PongHandler : IHandle<Pong>
    {
        public Action<IEnvelope> Handle( Pong message )
        {
            "Received Pong!".ToInfo<PingService>();
            return x => x.Acknowledge();
        }
    }
}
