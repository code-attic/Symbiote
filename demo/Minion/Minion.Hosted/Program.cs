using System;
using System.Collections.Generic;
using Minion.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Messaging;
using Symbiote.Rabbit;
using Symbiote.StructureMap;

namespace Minion.Hosted
{
    class Program
    {
        static void Main(string[] args)
        {
            var minion = new Minion();
            minion.Start( new Dictionary<string, object> {{ "args", args }} );
        }
    }
    [Serializable]
    public class Minion : MarshalByRefObject, IMinion
    {
        public void Start( IDictionary<string, object> startupData )
        {
            object val = null;
            var args = new string[] {};
            if(startupData != null && startupData.TryGetValue( "args", out val ))
            {
                args = (string[]) val;
            }

            try
            {
                Assimilate
                    .Core<StructureMapAdapter>()
                    .Messaging()
                    .AddConsoleLogger<IDaemon>(l => l.Info().MessageLayout(m => m.Message().Newline()))
                    .Daemon(x => x.Arguments(args))
                    .Rabbit(x => x.AddBroker(r => r.Defaults()))
                    .RunDaemon();
            }
            catch (Exception e)
            {
                Console.WriteLine( e );
            }
        }

        public void Stop()
        {
        }
    }

    public class Hosted : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            "Hosted Daemon has been summoned!"
                .ToInfo<IDaemon>();
            Bus.AddLocalChannel();
            Bus.AddRabbitChannel(x => x.Direct("Host").AutoDelete());

            Bus.AddRabbitChannel(x => x.Direct("Hosted").AutoDelete());
            Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Hosted").ExchangeName("Hosted").StartSubscription());

            Bus.Publish( "Host", new MinionUp() { Text = "You rang?"} );
        }

        public void Stop()
        {
            "Alas, I am dead fellah!"
                .ToInfo<IDaemon>();
        }

        public Hosted(IBus bus)
        {
            Bus = bus;
        }
    }

    public class CommandHandler : IHandle<MinionDoThis>
    {
        public void Handle( IEnvelope<MinionDoThis> envelope )
        {
            "Received: {0}".ToInfo<IDaemon>( envelope.Message.Text );
        }
    }
}
