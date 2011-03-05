// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.HttpListener
{
    public class HttpRequestTransform
    {
        public IRequest Transform( HttpListenerRequest origin )
        {
            var request = new Request();

            request.ClientEndpoint = origin.RemoteEndPoint;
            request.Headers = ProcessHeaders( origin.Headers );
            request.Method = origin.HttpMethod;
            request.Scheme = origin.Url.Scheme;
            request.Server = origin.Url.Host;
            request.RequestStream = origin.InputStream;
            request.BaseUri = origin.Url.GetLeftPart( UriPartial.Authority );
            request.RequestUri = origin.Url.PathAndQuery.Split( '?' )[0];
            request.FullUri = origin.Url;
            request.Version = origin.ProtocolVersion.ToString();

            request.Items = ProcessItems( request );

            return request;
        }

        public IDictionary<string, IEnumerable<string>> ProcessHeaders( NameValueCollection collection )
        {
            return collection
                .AllKeys.Select( x =>
                                     {
                                         var list = collection.GetValues( x );
                                         return Tuple.Create( x, list );
                                     } )
                .ToDictionary(
                    x => x.Item1,
                    x => (IEnumerable<string>) x.Item2 );
        }

        public IDictionary<string, object> ProcessItems( Request origin )
        {
            return new[]
                       {
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_METHOD, origin.Method ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_URI, origin.RequestUri ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_HEADERS, origin.Headers ),
                           Tuple.Create<string, object>( Owin.ItemKeys.BASE_URI, origin.BaseUri ),
                           Tuple.Create<string, object>( Owin.ItemKeys.SERVER_NAME, origin.Server ),
                           Tuple.Create<string, object>( Owin.ItemKeys.URI_SCHEME, origin.Scheme ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REMOTE_ENDPOINT, origin.ClientEndpoint ),
                           Tuple.Create<string, object>( Owin.ItemKeys.VERSION, origin.Version ),
                           Tuple.Create<string, object>( Owin.ItemKeys.REQUEST, origin ),
                           //Tuple.Create<string, object>( Owin.ItemKeys.REQUEST_BODY, ( (b, o, l, c, e) => origin.Read(b, o, l, c, e) ),
                       }.ToDictionary( x => x.Item1, x => x.Item2 );
        }
    }
}