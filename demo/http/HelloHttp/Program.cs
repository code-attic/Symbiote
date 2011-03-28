using System;
using System.Collections.Generic;
using Symbiote.Core;
using Symbiote.Daemon;
using Symbiote.Http;
using Symbiote.Http.Owin.Impl;
using Symbiote.Net;

namespace HelloHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            Assimilate
                                .Initialize()
                                .Daemon(x => x.Arguments(args))
                                .SocketServer( x => x.ListenOn( 8998 ) )
                                .HttpHost(x => x
                                                   //.ConfigureHttpListener( l => l.AddPort(8988))
                                                   .ConfigureSocketServer( s => { } )
                                                   .ConfigureWebAppSettings( w => w.BasePath( @"C:\git\Symbiote\demo\http\HelloHttp" ) )
                                                   .RegisterApplications( a => a.DefineApplication<IndexApp>(r => r.RequestUri == @"/" ) )
                                                   .RegisterApplications( a => a.DefineApplication<MessagingApplication>(r => r.RequestUri.StartsWith("/message")))
                                                   .RegisterApplications( a => a.DefineApplication<HelloHttpApp>(r => r.RequestUri.StartsWith("/hi")))
                                                   .RegisterApplications( a => a.DefineApplication<SayByeApp>(r => r.RequestUri.StartsWith("/bye")))
                                                   .RegisterApplications( a => a.DefineApplication<FileServer>(r => r.RequestUri.StartsWith("/file")))
                                                   .RegisterApplications( a => a.DefineApplication<PersonApp>( r => r.RequestUri.Equals(@"/haml") ) )
                                                   .RegisterApplications( a => a.DefineApplication<Empty>( r => true ) )
                                )
                                .RunDaemon();
        }
    }

    public class Empty : Application
    {
        public override bool HandleRequestSegment( ArraySegment<byte> data, Action continuation )
        {
            return false;
        }

        public override void OnError( Exception exception )
        {
            
        }

        public override void CompleteResponse()
        {
            Response
                .Submit( HttpStatus.NoContent );
        }
    }

    public class PersonApp : Application
    {
        public override bool HandleRequestSegment( ArraySegment<byte> data, Action continuation )
        {
            return false;
        }

        public override void OnError( Exception exception )
        {
            
        }

        public override void CompleteResponse()
        {
            Response
                .RenderView( new Person() { Id = "1", Name = "Dude Man" }, "Detail" )
                .Submit( HttpStatus.Ok );
        }
    }

    public class IndexApp : Application
    {
        public override bool HandleRequestSegment( ArraySegment<byte> data, Action continuation )
        {
            return false;
        }

        public override void OnError( Exception exception )
        {
            
        }

        public override void CompleteResponse()
        {
            Response
                .AppendToBody( "You should fiddle around with the site. It's fun." )
                .Submit( HttpStatus.Ok );
        }
    }

    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}