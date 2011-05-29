using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using pingpong.messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Messaging;
using Symbiote.Rabbit;

namespace pong
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .Rabbit( x => x.AddBroker( b => b.Defaults() ) )
                .AddConsoleLogger<PongService>( x => x.Debug().MessageLayout( m => m.Message().Newline() ) )
                .Daemon( x => x.Name( "pong" ) )
                .RunDaemon();
        }
    }

    public class PongService : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Bus.AddRabbitChannel( x => x.Direct( "ping" ) );
            Bus.AddRabbitQueue( x => x.ExchangeName( "ping" ).QueueName( "ping" ).NoAck().StartSubscription() );
        }

        public void Stop()
        {
            
        }

        public PongService( IBus bus )
        {
            Bus = bus;
        }
    }

    public class PingHandler : IHandle<Ping>
    {
        public Action<IEnvelope> Handle( Ping message )
        {
            "Received Ping!".ToInfo<PongService>();
            return x => x.Reply( new Pong() );
        }
    }
}
