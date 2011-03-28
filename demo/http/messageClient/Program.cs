using System;
using System.Diagnostics;
using Messages;
using Symbiote.Core;
using Symbiote.Messaging;
using Symbiote.Daemon;
using Symbiote.Http;
using Symbiote.Net;
using Symbiote.StructureMapAdapter;

namespace messageClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
				.SocketServer( s => s.ListenOn( 8998 ) )
				.HttpHost( h => h.ConfigureSocketServer( s => { } ) )
                .Daemon( x => x.Arguments( args ) )
                .RunDaemon();
        }
    }

    public class Service : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Bus.AddHttpChannel( x => x.Name("http").BaseUrl( @"http://localhost:8989/message" ));
            var watch = Stopwatch.StartNew();

            for (int i = 0; i < 100; i++)
            {
                Message response = Bus.Request<Message, Message>("http", new Message("Hi, server"));
                Console.WriteLine("Response received in {0}: '{1}'", watch.ElapsedMilliseconds, response.Text);    
            }
            Console.WriteLine( "{0} ms avg", watch.ElapsedMilliseconds / 100 );
            watch.Stop();
        }

        public void Stop()
        {
            
        }

        public Service( IBus bus )
        {
            Bus = bus;
        }
    }
}
