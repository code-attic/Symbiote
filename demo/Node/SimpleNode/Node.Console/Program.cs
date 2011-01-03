using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Messaging.Impl.Mesh;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Log4Net;
using Symbiote.Core.Extensions;
using Symbiote.Rabbit;
using Timer = System.Timers.Timer;

namespace Node.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Daemon( x => x.Arguments( args ).Name( "node" ) )
                .Messaging()
                .Rabbit(x => x.AddBroker(b => b.Address("localhost").AMQP091()).EnrollAsMeshNode())
                .AddConsoleLogger<NodeService>( x => x.Debug().MessageLayout( m => m.Message().Newline() ) )
                .RunDaemon();
        }
    }

    public class Message
    {
        public string Text { get; set; }

        public Message() {}

        public Message( string text )
        {
            Text = text;
        }
    }

    public class MessageHandler
        : IHandle<Message>
    {
        public INodeIdentityProvider IdentityProvider { get; set; }

        public void Handle( IEnvelope<Message> envelope )
        {
            "{0} got a message: {1}"
                .ToDebug<NodeService>(IdentityProvider.Identity, envelope.Message);
        }

        public MessageHandler( INodeIdentityProvider identityProvider )
        {
            IdentityProvider = identityProvider;
        }
    }

    public class NodeService
        : IDaemon
    {
        public INode Node { get; set; }
        public INodeIdentityProvider IdentityProvider { get; set; }
        public Timer Timer { get; set; }

        public void Start()
        {
            "Starting node {0}"
                .ToDebug<NodeService>(IdentityProvider.Identity);

            while (true)
            {
                Node.Publish( "Dis here's a message from '{0}'".AsFormat( IdentityProvider.Identity ) );
                Thread.Sleep( 1000 );
            }
        }

        public void Stop()
        {
            "Stopping node {0}"
                .ToDebug<NodeService>(IdentityProvider.Identity);
        }

        public NodeService(INode node, INodeIdentityProvider identityProvider)
        {
            Node = node;
            IdentityProvider = identityProvider;
            this.Timer = new Timer(1000);
            this.Timer.Start();
            this.Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Node.Publish( new Message("Hi, from ") );
        }
    }
}
