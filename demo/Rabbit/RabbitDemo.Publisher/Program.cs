using System.Diagnostics;
using System.Linq;
using RabbitDemo.Messages;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.StructureMapAdapter;
using Symbiote.Messaging;
using Symbiote.Rabbit;
using Symbiote.Log4Net;
using Symbiote.Daemon;

namespace RabbitDemo.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .Rabbit(x => x.AddBroker(r => r.Defaults()))
                .AddConsoleLogger<Publisher>(x => x.Info().MessageLayout(m => m.TimeStamp().Message().Newline()))
                .AddConsoleLogger<IDaemon>(x => x.Info().MessageLayout(m => m.TimeStamp().Message().Newline()))
                .Daemon(x => x.Name("publisher").Arguments(args))
                .RunDaemon();
        }
    }

    public class Publisher
        : IDaemon
    {
        public IBus Bus { get; set; }
        public int MessageCount { get; set; }
        public int ActorCount { get; set; }

        public void Start()
        {
            "Starting Publisher".ToInfo<Publisher>();
            "Configuring Rabbit...".ToInfo<Publisher>();
            Bus.AddRabbitChannel(x => x.Fanout("test").AutoDelete().PersistentDelivery().CorrelateBy<Message>( m => m.CorrelationId ));

            "Creating {0} messages for {1} actors...".ToInfo<Publisher>(MessageCount, ActorCount);
            var messages = Enumerable.Range(1, MessageCount)
                .Select(x => new Message((x%ActorCount).ToString(), x))
                .ToList();

            "Sending Messages".ToInfo<Publisher>();
            var watch = Stopwatch.StartNew();
            var count = 0;
            messages
                .ForEach(x =>
                {
                    Bus.Publish( "test", x );
                    //if(++count%1000==0)
                    //    Bus.CommitChannelOf<Message>();
                } );
                //.AsParallel()
                //.ForAll(x => Bus.Publish("test", x));
            watch.Stop();
            "{0} messages sent in {1} seconds"
                .ToInfo<Publisher>(MessageCount, watch.Elapsed.TotalSeconds);
        }

        public void Stop()
        {
            
        }

        public Publisher(IBus bus)
        {
            Bus = bus;
            MessageCount = 100000;
            ActorCount = 100;
        }
    }
}
