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
using System.IO;
using System.Web;
using Symbiote.Core.Impl.Futures;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.Web
{
    public class HttpRequestAdapter : IRequest
    {
        protected HttpRequest Request { get; set; }

        public string Method
        {
            get { throw new NotImplementedException(); }
        }
        public string Uri
        {
            get { throw new NotImplementedException(); }
        }
        public string Url
        {
            get { throw new NotImplementedException(); }
        }
        public IDictionary<string, string> Parameters
        {
            get { throw new NotImplementedException(); }
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

        public HttpRequestAdapter(HttpRequest request)
        {
            Request = request;
        }
    }
}