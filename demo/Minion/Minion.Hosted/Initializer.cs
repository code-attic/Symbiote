using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Rabbit;

namespace Minion.Hosted
{
    public class Initializer : IMinion
    {
        public void Initialize() 
        {
            DaemonAssimilation.RunDaemon( Assimilate
                                .Initialize()
                                .Daemon(x => x.Arguments(new string[]{}))
                                .AddConsoleLogger<IMinion>(l => l.Info().MessageLayout(m => m.Message().Newline()))
                                .AddColorConsoleLogger<IDaemon>( l => l.Info().MessageLayout( m => m.Message().Newline() ).DefineColor().Text.IsWhite().BackGround.IsRed().ForAllOutput() )
                                .Rabbit(x => x.AddBroker(r => r.Defaults())) );
        }
    }
}