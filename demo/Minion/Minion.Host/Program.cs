using System.Diagnostics;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Rabbit;

namespace Minion.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Initialize()
                .AddConsoleLogger<IMinion>( l => l.Info().MessageLayout( m => m.Message().Newline() ) )
                .AddColorConsoleLogger<IDaemon>( l => l.Info().MessageLayout( m => m.Message().Newline() ).DefineColor().Text.IsGreen().BackGround.IsWhite().ForAllOutput() )
                .Daemon( x => x.Arguments( args ).AsDynamicHost( b => b.HostApplicationsFrom( @"C:\active-git\Symbiote\demo\Minion\Minions" ) ) )
                .Rabbit( x => x.AddBroker( r => r.Defaults() ) )
                .RunDaemon();
        }
    }
}
