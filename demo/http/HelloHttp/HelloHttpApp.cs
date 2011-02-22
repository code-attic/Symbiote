using System;
using System.Collections.Generic;
using Symbiote.Http;
using Symbiote.Http.Impl;

namespace HelloHttp
{
    public class HelloHttpApp : IApplication
    {
        public void Process(IDictionary<string, object> requestItems, OwinResponse respond, Action<Exception> onException)
        {
            respond
                .Build()
                .DefineHeaders( x => x.ContentType( ContentType.Plain ) )
                .AppendToBody( "Hi http, this is an OWIN compliant host!" )
                .Submit( HttpStatus.Ok );
        }
    }
}