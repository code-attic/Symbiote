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
using System.IO;
using System.Net;
using Symbiote.Core.Futures;
using Symbiote.Http.Owin.Impl;

namespace Symbiote.Http.Owin
{
    public interface IRequest
    {
        IPEndPoint ClientEndpoint { get; }
        string Method { get; }
        string BaseUri { get; }
        string RequestUri { get; }
        List<string> PathSegments { get; }
        string Scheme { get; }
        string Server { get; }
        string Version { get; }
        bool Valid { get; }
        bool HeadersComplete { get; }

        IDictionary<string, string> Parameters { get; }
        IDictionary<string, string> Headers { get; }
        IDictionary<string, object> Items { get; }
        OwinBody Body { get; set; }
        ParseRequestSegment Parse { get; set; }
        Action<ArraySegment<byte>> OnBody { get; set; }
    }
}