using System;
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
                                .Initialize()
                                .Daemon(x => x.Arguments(args))
                                .HttpHost(x => x
                                                   //.ConfigureHttpListener( l => l.AddPort(8988))
                                                   .ConfigureSocketServer( s => s.Port( 8988 ) )
                                                   .ConfigureWebAppSettings( w => w.BasePath( @"C:\git\Symbiote\demo\http\HelloHttp" ) )
                                                   .RegisterApplications( a => a.DefineApplication(r => r.Url.Equals(@"/"), (rq, rsp, ex) => rsp.Build().AppendToBody( "You should fiddle around with the site. It's fun." ).Submit( HttpStatus.Ok )))
                                                   .RegisterApplications( a => a.DefineApplication<MessagingApplication>(r => r.Url.StartsWith("/message")))
                                                   .RegisterApplications( a => a.DefineApplication<HelloHttpApp>(r => r.Url.StartsWith("/hi")))
                                                   .RegisterApplications( a => a.DefineApplication<SayByeApp>(r => r.Url.StartsWith("/bye")))
                                                   .RegisterApplications( a => a.DefineApplication<FileServer>(r => r.Url.StartsWith("/file")))
                                                   .RegisterApplications( a => a.DefineApplication<PersonApp>( r => r.Url.Equals(@"/haml") ) )
                                                   .RegisterApplications( a => a.DefineApplication((r => true), (rq, rsp, ex) => rsp(Owin.HttpStatus.NO_CONTENT, new Dictionary<string, string>(), new string[]{})))
                                ).RunDaemon();
        }
    }

    public class PersonApp : IApplication
    {
        public void Process( IDictionary<string, object> requestItems, OwinResponse respond, Action<Exception> onException )
        {
            respond
                .Build()
                .RenderView( new Person() { Id = "1", Name = "Dude Man" }, "Detail" )
                .Submit( HttpStatus.Ok );
        }
    }

    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}