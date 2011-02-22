using System;
using System.Collections.Generic;
using Nancy;
using Symbiote.Http;
using Symbiote.Http.Impl;
using Symbiote.Nancy.Impl;
using Symbiote.Core.Extensions;

namespace Symbiote.Nancy
{
    public class NancyApplication : IApplication
    {
        public INancyEngine Engine { get; set; }

        public static long requests;

        public void Process( IDictionary<string, object> requestItems, OwinResponse respond, Action<Exception> onException )
        {
            "Request {0}".ToInfo<IApplication>( ++requests );
            var request = requestItems.ToNancyRequest();
            var response = Engine.HandleRequest( request );

            HttpStatus status = response.StatusCode;
            respond( status.ToString(), response.Headers, new[] {response.Contents} );
        }

        public NancyApplication( INancyEngine engine )
        {
            Engine = engine;
        }
    }
}
