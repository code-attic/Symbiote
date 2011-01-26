
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Http.Impl;
using Symbiote.Http.Owin;
using Symbiote.StructureMap;
using Symbiote.Daemon;
using Symbiote.Messaging;
using Symbiote.JsonRpc.Host;
using Symbiote.Actor;

namespace HelloHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                .Core<StructureMapAdapter>()
                .Daemon(x => x.Arguments(args))
                .Actors()
                .Messaging()
                .HttpHost(x => x
                    .ConfigureHttpListener(l => l.AddPort(8989))
                    .RegisterApplications(a => a.DefineApplication(r => r.Url.Equals(@"/"), (rq, rsp, ex) => rsp(Owin.HttpStatus.OK,
                                                                                                                new Dictionary<string, IList<string>>(),
                                                                                                                new[]
                                                                                                                    {
                                                                                                                        "This is fun.",
                                                                                                                        " But you should really do something more interesting ;)"
                                                                                                                    })))
                    .RegisterApplications(a => a.DefineApplication<HelloHttpApp>(r => r.Url.StartsWith("/hi")))
                    .RegisterApplications(a => a.DefineApplication<SayByeApp>(r => r.Url.StartsWith("/bye")))
                    .RegisterApplications(a => a.DefineApplication<FileServer>(r => r.Url.StartsWith("/file")))
                    .RegisterApplications(a => a.DefineApplication((r => true), (rq, rsp, ex) => rsp(Owin.HttpStatus.NO_CONTENT, new Dictionary<string, IList<string>>(), new string[]{})))
                    )
                .RunDaemon();
        }
    }

    public class SayByeApp : IApplication
    {
        public void Process(IDictionary<string, object> requestItems, Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond, Action<Exception> onException)
        {
            respond
                (
                    Owin.HttpStatus.OK, 
                    new Dictionary<string, IList<string>>(), 
                    new[] { "See ya, {0}!".AsFormat( "dude" )}
                );
        }
    }

    public class HelloHttpApp : IApplication
    {
        public void Process(IDictionary<string, object> requestItems, Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond, Action<Exception> onException)
        {
            respond(
                Owin.HttpStatus.OK,
                new Dictionary<string, IList<string>>(),
                new[] { "Hi Http, this is a OWIN compliant host!" });
        }
    }

    public class FileServer : IApplication
    {
        public void Process(IDictionary<string, object> requestItems, Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond, Action<Exception> onException)
        {
            var url = requestItems[Owin.ItemKeys.REQUEST_URI].ToString().Replace("/", @"\");
            var filePath = @"..\.." + url;

            if(!File.Exists(filePath))
            {
                respond(
                    Owin.HttpStatus.NOTFOUND,
                    new Dictionary<string, IList<string>>()
                        {
                            {"content-type", new List<string>() {"text/html"}}
                        },
                    new[] { "DAMMIT JIM, I'm a file server, not a magician!" });
            }

            using(var memoryStream = new MemoryStream())
            using(var stream = new FileStream(filePath, FileMode.Open ))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int) stream.Length);
             
                respond(
                    Owin.HttpStatus.OK,
                    new Dictionary<string, IList<string>>()
                        {
                            {"content-type", new List<string>() {"text/html"}}
                        },
                    new [] {bytes});
            }
        }
    }

    public class HostService : IDaemon
    {
        public IHost Host { get; set; }

        public void Start()
        {
            Host.Start();
        }

        public void Stop()
        {
            Host.Stop();
        }

        public HostService(IHost host)
        {
            Host = host;
        }
    }
}
