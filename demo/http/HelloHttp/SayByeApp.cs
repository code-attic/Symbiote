using System;
using System.Collections.Generic;
using Symbiote.Http;
using Symbiote.Http.Owin.Impl;

namespace HelloHttp
{
    public class SayByeApp : Application
    {
        public override bool OnNext( ArraySegment<byte> data, Action continuation )
        {
            return false;
        }

        public override void OnError( Exception exception )
        {
            
        }

        public override void OnComplete()
        {
            Response
                .DefineHeaders( x => x.ContentType( ContentType.Plain ) )
                .AppendToBody( "See ya!" )
                .Submit( HttpStatus.Ok );
        }
    }
}