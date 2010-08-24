using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Jackalope;
using Symbiote.Log4Net;
using Symbiote.Daemon;
using Symbiote.StructureMap;

namespace SubscribeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .AddColorConsoleLogger<IBus>(x => 
                    x.Info()
                    .MessageLayout(m => m.Message().Newline())
                    .DefineColor()
                        .Text.IsHighIntensity().BackGround.IsRed().ForAllOutput())
                .AddConsoleLogger<Subscriber>(x => x.Info().MessageLayout(m => m.Message().Newline()))
                .Daemon(x => x.Arguments(args).DisplayName("Subscriber Demo").Description("A subscriber").Name("Subscriber"))
                .Jackalope(x => x.AddServer(s => s.AMQP08().Address("localhost")))
                .RunDaemon();
        }
    }
}
