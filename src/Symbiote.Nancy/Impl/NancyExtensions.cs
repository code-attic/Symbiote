using System.Collections.Generic;
using System.Linq;
using Nancy;
using Symbiote.Core.Extensions;
using Symbiote.Core.Utility;
using Symbiote.Http;

namespace Symbiote.Nancy.Impl
{
    public static class NancyExtensions
    {
        public static Request ToNancyRequest( this IDictionary<string, object> requestItems )
        {
            var request = requestItems.ExtractRequest();
            var parameters = DelimitedBuilder.Construct( request.Parameters.Select( x => "{0}={1}".AsFormat( x.Key, x.Value ) ), "&" );
            return new Request( request.Method, request.RequestUri, request.Headers, request.RequestStream, request.Scheme, parameters );
        }
    }
}