/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Net;
using Symbiote.Core.Impl.Futures;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.Web
{
    public class HttpListenerRequestAdapter : IRequest
    {
        protected HttpListenerRequest Request { get; set; }

        public string Method
        {
            get { return Request.HttpMethod; }
        }
        public string Uri
        {
            get { return Request.Url.PathAndQuery; }
        }
        public IDictionary<string, IEnumerable<string>> Headers
        {
            get { throw new NotImplementedException(); }
        }
        public IDictionary<string, object> Items
        {
            get { throw new NotImplementedException(); }
        }
        public Future<int> Read( byte[] buffer, int offset, int count, Func<byte[], int, int, int> callback, object state )
        {
            throw new NotImplementedException();
        }

        public HttpListenerRequestAdapter(HttpListenerRequest request)
        {
            Request = request;
        }
    }
}