using System;
using System.Linq;
using System.Text;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Relax;
using Symbiote.Log4Net;
using Symbiote.Eidetic;

namespace RelaxDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core()
                .Daemon(x => x
                           .Arguments(args)
                           .Name("relaxdemo")
                           .DisplayName("Relax Demo")
                           .Description("Relax Integration Testing")
                )
                .Eidetic()
                .Relax(x => x.UseDefaults().Cache())
                .AddColorConsoleLogger<ChangeWatcher>(x => x
                                                               .Info()
                                                               .DefineColor()
                                                               .Text.IsGreen().ForAllOutput()
                                                               .MessageLayout(m => m.Message().Date().Newline())
                )
                .AddConsoleLogger<RelaxDemoService>(x => x
                                                             .Info()
                                                             .MessageLayout(m => m.Message().Newline())
                )
                .RunDaemon();
        }
    }
}
