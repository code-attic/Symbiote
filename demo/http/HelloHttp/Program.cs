using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Http;
using Symbiote.Http.Impl;
using Symbiote.Messaging;
using Symbiote.StructureMapAdapter;
using DaemonAssimilation = Symbiote.Daemon.DaemonAssimilation;

namespace HelloHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                                .Core<StructureMapAdapter>()
                                .Daemon(x => x.Arguments(args))
                                .Messaging()
                                .HttpHost(x => x
                                                   .ConfigureHttpListener(l => l.AddPort(8988))
                                                   .RegisterApplications(a => a.DefineApplication(r => r.Url.Equals(@"/"), (rq, rsp, ex) => rsp.Build().AppendToBody( "You should fiddle around with the site. It's fun." ).Submit( HttpStatus.Ok )))
                                                   .RegisterApplications(a => a.DefineApplication<MessagingApplication>(r => r.Url.StartsWith("/message")))
                                                   .RegisterApplications(a => a.DefineApplication<HelloHttpApp>(r => r.Url.StartsWith("/hi")))
                                                   .RegisterApplications(a => a.DefineApplication<SayByeApp>(r => r.Url.StartsWith("/bye")))
                                                   .RegisterApplications(a => a.DefineApplication<FileServer>(r => r.Url.StartsWith("/file")))
                                                   .RegisterApplications(a => a.DefineApplication((r => true), (rq, rsp, ex) => rsp(Owin.HttpStatus.NO_CONTENT, new Dictionary<string, string>(), new string[]{}))))
                                .RunDaemon();
        }
    }
}