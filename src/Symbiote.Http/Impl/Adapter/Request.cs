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
using System.Collections.Generic;
using System.IO;
using System.Net;
using Symbiote.Core.Extensions;
using Symbiote.Core.Futures;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter
{
    public class Request
        : IRequest
    {
        public Stream Stream { get; set; }

        public IPEndPoint ClientEndpoint { get; set; }
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Server { get; set; }
        public string Uri { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public IDictionary<string, object> Items { get; set; }

        public Future<byte[]> Read()
        {
            return Future
                .Of( () => Stream.ReadToEnd( 1000 ) );
        }

        public Request()
        {
            Parameters = new Dictionary<string, string>();
            Headers = new Dictionary<string, IEnumerable<string>>();
            Items = new Dictionary<string, object>();
        }
    }
}