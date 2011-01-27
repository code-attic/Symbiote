using System.Collections.Generic;
using Symbiote.Actor;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Http;
using Symbiote.Http.Impl;
using Symbiote.Messaging;
using Symbiote.StructureMap;
using DaemonAssimilation = Symbiote.Daemon.DaemonAssimilation;

namespace HelloHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            DaemonAssimilation.RunDaemon( Assimilate
                                .Core<StructureMapAdapter>()
                                .Daemon(x => x.Arguments(args))
                                .Actors()
                                .Messaging()
                                .HttpHost(x => x
                                                   .ConfigureHttpListener(l => l.AddPort(8989))
                                                   .RegisterApplications(a => a.DefineApplication(r => r.Url.Equals(@"/"), (rq, rsp, ex) => rsp.Build().AppendToBody( "You should fiddle around with the site. It's fun." ).Submit( HttpStatus.Ok )))
                                                   .RegisterApplications(a => a.DefineApplication<HelloHttpApp>(r => r.Url.StartsWith("/hi")))
                                                   .RegisterApplications(a => a.DefineApplication<SayByeApp>(r => r.Url.StartsWith("/bye")))
                                                   .RegisterApplications(a => a.DefineApplication<FileServer>(r => r.Url.StartsWith("/file")))
                                                   .RegisterApplications(a => a.DefineApplication((r => true), (rq, rsp, ex) => rsp(Owin.HttpStatus.NO_CONTENT, new Dictionary<string, IList<string>>(), new string[]{})))
                                ) );
        }
    }
}