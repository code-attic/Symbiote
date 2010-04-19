using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.SocketMQ.Impl;
using Symbiote.WebSocket;
using Symbiote.Jackalope;
using Symbiote.SocketMQ;

namespace SocketMQ.ChatService
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core()
                .Daemon(
                    x => x.Name("wss").DisplayName("Web Socket Service").Description("A web socket service").Arguments(args))
                .WebSocketServer(x =>
                    x.ServerUrl(@"http://localhost:8080")
                     .SocketServer("localhost")
                     .SocketResource("chat")
                     .Port(8181)
                     .PermitFlashSocketConnections())
                .Jackalope(x => x.AddServer(s => s.AMQP08()))
                .SocketMQ()
                .AddConsoleLogger<ChatService>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .AddColorConsoleLogger<ISocketServer>(x => x.Info().MessageLayout(m => m.Message().Newline()).DefineColor().Text.IsYellow().ForAllOutput())
                .AddColorConsoleLogger<ClientMessage>(x => x.Info().MessageLayout(m => m.Message().Newline()).DefineColor().Text.IsBlue().ForAllOutput())
                .AddFileLogger<ISocketServer>(x => x.Info().MessageLayout(m => m.Message().Newline()).FileName("log.log"))
                .AddFileLogger<IMessageGateway>(x => x.Info().MessageLayout(m => m.Message().Newline()).FileName("socketmq.log"))
                .RunDaemon();
        }
    }
}
