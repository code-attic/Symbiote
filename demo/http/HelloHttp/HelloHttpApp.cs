using System;
using System.Collections.Generic;
using Symbiote.Http;
using Symbiote.Http.Owin.Impl;

namespace HelloHttp
{
    public class HelloHttpApp : Application
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
                .DefineHeaders( x => x.ContentType( ContentType.Plain ) )
                .AppendToBody( "Hi http, this is an OWIN compliant host!" )
                .Submit( HttpStatus.Ok );
        }
    }
}