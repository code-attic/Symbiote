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
            //request.RequestStream = origin.InputStream;
            request.BaseUri = origin.Url.GetLeftPart( UriPartial.Authority );
            request.RequestUri = origin.Url.PathAndQuery.Split( '?' )[0];
            request.PathSegments = origin.Url.Segments.ToList();
            request.Version = origin.ProtocolVersion.ToString();

            request.Items = request.GetOwinDictionary();

            return request;
        }

        public IDictionary<string, string> ProcessHeaders( NameValueCollection collection )
        {
            return collection
                .AllKeys
                .ToDictionary(
                            x => x,
                            x => string.Join( ",", collection.GetValues( x ) )
                );
        }
    }
}