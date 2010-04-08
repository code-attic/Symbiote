using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Warren;
using Symbiote.WebSocket;
using Symbiote.Jackalope;

namespace WarrenService
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core()
                .Daemon<WarrenService>(
                    x => x.Name("wss").DisplayName("Web Socket Service").Description("A web socket service").Arguments(args))
                .WebSocketServer(x =>
                    x.ServerUrl(@"http://localhost:8080")
                     .SocketUrl(@"ws://localhost:8181/chat")
                     .Port(8181))
                .Jackalope(x => x.AddServer(s => s.AMQP08()))
                .AddConsoleLogger<WarrenService>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddColorConsoleLogger<ISocketServer>(x => x.Info().MessageLayout(m => m.Message().Newline()).DefineColor().Text.IsYellow().ForAllOutput())
                .AddColorConsoleLogger<ClientMessage>(x => x.Info().MessageLayout(m => m.Message().Newline()).DefineColor().Text.IsBlue().ForAllOutput())
                .AddFileLogger<ISocketServer>(x => x.Info().MessageLayout(m => m.Message().Newline()).FileName("log.log"))
                .AddFileLogger<Bridge>(x => x.Info().MessageLayout(m => m.Message().Newline()).FileName("warren.log"))
                .RunDaemon<WarrenService>();
        }
    }
}
