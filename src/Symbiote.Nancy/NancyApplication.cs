using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
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

        public void Process( IDictionary<string, object> requestItems, Action<string, IDictionary<string, IList<string>>, IEnumerable<object>> respond, Action<Exception> onException )
        {
            "Request {0}".ToInfo<IApplication>( ++requests );
            var request = requestItems.ToNancyRequest();
            var response = Engine.HandleRequest( request );

            var responseHeaders =
                (IDictionary<string, IList<string>>) 
                response.Headers
                    .ToDictionary( x => x.Key, x => (IList<string>) x.Value.Split( ';' ).ToList() );

            var status = HttpStatus.CodeLookup[(int)response.StatusCode].ToString();

            using(var stream = new MemoryStream())
            {
                response.Contents( stream );
                respond(
                    status,
                    responseHeaders,
                    new [] { stream.ToArray() }
                );
            }
        }

        public NancyApplication( INancyEngine engine )
        {
            Engine = engine;
        }
    }
}
