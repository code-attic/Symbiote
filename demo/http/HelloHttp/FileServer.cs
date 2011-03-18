using System;
using Symbiote.Http;
using Symbiote.Http.Owin.Impl;

namespace HelloHttp
{
    public class FileServer : Application
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
                .AppendFileContentToBody( Request.RequestUri )
                .Submit( HttpStatus.Ok );

        }
    }
}
