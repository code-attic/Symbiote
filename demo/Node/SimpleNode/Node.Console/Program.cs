using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Rabbit;

namespace Node.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .Daemon( x => x.Arguments( args ).Name( "node" ) )
                .Rabbit(x => x.AddBroker( b => b.Address( "localhost" ).AMQP091() ).EnrollAsMeshNode(false) )
                .AddConsoleLogger<NodeService>( x => x.Debug().MessageLayout( m => m.Message().Newline() ) )
                .RunDaemon();
        }
    }
}
