using System;
using Messages;
using Symbiote.Core;
using Symbiote.StructureMap;
using Symbiote.Actor;
using Symbiote.Messaging;
using Symbiote.Daemon;
using Symbiote.Http;

namespace messageClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Actors()
                .Messaging()
                .HttpHost( x => x.ConfigureHttpListener( s => {} ) )
                .Daemon( x => x.Arguments( args ) )
                .RunDaemon();
        }
    }

    public class Service : IDaemon
    {
        public IBus Bus { get; set; }

        public void Start()
        {
            Bus.AddHttpChannel( x => x.Name("http").BaseUrl( "http://localhost:8989/message" ));

            Message response = Bus.Request<Message, Message>( "http", new Message( "Hi, server" ) );

            Console.WriteLine("Response received: {0}", response.Text);
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
