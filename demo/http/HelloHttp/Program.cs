
using System;
using System.Collections.Generic;
using Symbiote.Core;
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
                .HttpHost(x => x.ConfigureHttpListener(l => l.AddPort(8989)).RegisterApplications(a => a.DefineApplication<HelloHttpApp>(r => true)))
                .RunDaemon();
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
