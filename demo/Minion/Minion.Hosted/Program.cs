using System;
using System.Collections.Generic;
using Minion.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Messaging;
using Symbiote.Rabbit;

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
                    .Initialize()
                    .Daemon(x => x.Arguments(args))
                    .AddConsoleLogger<IMinion>(l => l.Info().MessageLayout(m => m.Message().Newline()))
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
                .ToInfo<IMinion>();
            Bus.AddLocalChannel();
            Bus.AddRabbitChannel(x => x.Direct("Host").AutoDelete());

            Bus.AddRabbitChannel(x => x.Direct("Hosted").AutoDelete());
            Bus.AddRabbitQueue(x => x.AutoDelete().QueueName("Hosted").ExchangeName("Hosted").StartSubscription().NoAck());

            Bus.Publish( "Host", new MinionUp() { Text = "You rang?"} );
        }

        public void Stop()
        {
            "Alas, I am dead fellah!"
                .ToInfo<IMinion>();
        }

        public Hosted(IBus bus)
        {
            Bus = bus;
        }
    }

    public class CommandHandler : IHandle<MinionDoThis>
    {
        public Action<IEnvelope> Handle( MinionDoThis message )
        {
            "Received: {0}".ToInfo<IMinion>( message.Text );
            return x => x.Acknowledge();
        }
    }
}
