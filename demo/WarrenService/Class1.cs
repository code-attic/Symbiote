using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Core.Extensions;
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
                .RunDaemon<WarrenService>();
        }
    }

    public class MessageHandler : IMessageHandler<string>
    {
        protected IBus _bus;

        public void Process(string message, IResponse response)
        {
            try
            {
                "Client sez: {0}".ToInfo<ClientMessage>(message);
                response.Acknowledge();

                _bus.Send("client", new SocketMessage() {Body = "If you ever speak to me that way again. I'll kill your face.", To = response.FromQueue});
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }

        public MessageHandler(IBus bus)
        {
            _bus = bus;
        }
    }

    public class ClientMessage
    {
        public string Body { get; set; }
    }

    public class WarrenService : IDaemon
    {
        protected Bridge _bridge;
        protected IBus _bus;

        public void Start()
        {
            _bridge.Start();
        }

        public void Stop()
        {
            
        }

        public WarrenService(Bridge bridge, IBus bus)
        {
            _bridge = bridge;
            _bus = bus;

            _bus.AddEndPoint(x => x.Exchange("server", ExchangeType.direct).QueueName("serverQueue"));
            _bus.AddEndPoint(x => x.Exchange("client", ExchangeType.fanout));

            _bus.Subscribe("serverQueue", null);
        }
    }
}
