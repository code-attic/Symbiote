using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Log4Net;
using Symbiote.Eidetic;
using Symbiote.Jackalope;
using StructureMap;
using Symbiote.Relax;

namespace DaemonDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Assimilate
                .Core()
                .Daemon<Demo>(d => d
                    .Name("demo.daemon")
                    .Description("daemon demo")
                    .DisplayName("demo")
                    .Arguments(args)
                    )
                .Eidetic(x => x.AddServer("localhost", 11211))
                .Relax(x => x
                    .Server("localhost")
                    .UseForType<JsonDocument>("general")
                    .UseForType<ExchangeRecord>("exchanges"))
                .Jackalope(x => x.AddServer(s => s.AMQP08().Address("localhost")))
                .AddConsoleLogger<Demo>(c => c.Info().MessageLayout(m => m.Message().Newline()))
                //.AddConsoleLogger<IBus>(c => c.Error().MessageLayout(m => m.Message().Newline()))
                .RunDaemon<Demo>();
        }
    }
}
