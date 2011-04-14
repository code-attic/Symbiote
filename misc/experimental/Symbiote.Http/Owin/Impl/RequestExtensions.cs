using System;
using System.Collections.Generic;
using Symbiote.Core;

namespace Symbiote.Http.Owin.Impl
{
    public static class RequestExtensions
    {
        public static IDictionary<string, object> GetOwinDictionary( this IRequest request )
        {
            return new Dictionary<string, object>()
                       {
                           { Owin.ItemKeys.REQUEST_METHOD, request.Method },   
                           { Owin.ItemKeys.REQUEST_URI, request.RequestUri },
                           { Owin.ItemKeys.REQUEST_HEADERS, request.Headers },
                           { Owin.ItemKeys.BASE_URI, request.BaseUri },
                           { Owin.ItemKeys.SERVER_NAME, request.Server },
                           { Owin.ItemKeys.URI_SCHEME, request.Scheme },
                           { Owin.ItemKeys.REMOTE_ENDPOINT, request.ClientEndpoint },
                           { Owin.ItemKeys.VERSION, request.Version },
                           { Owin.ItemKeys.REQUEST, request },
                           { Owin.ItemKeys.REQUEST_BODY, request.Body },
                       };
        }
    }
}