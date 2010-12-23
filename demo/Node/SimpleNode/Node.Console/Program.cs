using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Messaging.Impl.Mesh;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Log4Net;
using Symbiote.Core.Extensions;

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
                .AsNode()
                .AddConsoleLogger<NodeService>( x => x.Debug().MessageLayout( m => m.Message().Newline() ) )
                .RunDaemon();
        }
    }

    public class NodeService
        : IDaemon
    {
        public INode Node { get; set; }

        public void Start()
        {
            "Starting node..."
                .ToDebug<NodeService>();
        }

        public void Stop()
        {
            "Stopping node..."
                .ToDebug<NodeService>();
        }

        public NodeService( INode node )
        {
            Node = node;
        }
    }
}
