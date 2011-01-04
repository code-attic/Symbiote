using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.StructureMap;
using Symbiote.Messaging;
using Symbiote.Log4Net;
using Symbiote.Rabbit;

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
}
