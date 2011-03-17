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
using System.Net;
using Symbiote.Http.NetAdapter.SocketListener;

namespace Symbiote.Http.Owin.Impl
{
    public class Request
        : IRequest
    {
        public bool Initialized { get; set; }
        public IPEndPoint ClientEndpoint { get; set; }
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Server { get; set; }
        public string BaseUri { get; set; }
        public string RequestUri { get; set; }
        public List<string> PathSegments { get; set; }
        public string Version { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public IDictionary<string, object> Items { get; set; }
        public Queue<ArraySegment<byte>> RequestChunks { get; set; }
        public Action<ArraySegment<byte>> OnData { get; set; }
        public Action<Exception> OnException { get; set; }

        public void BytesReceived( byte[] bytes )
        {
            OnData( new ArraySegment<byte>( bytes ) );
            OnData = CacheBytes;
        }

        public void CacheBytes( ArraySegment<byte> bytes )
        {
            RequestChunks.Enqueue( bytes );
        }

        public void ReadNext( Action<ArraySegment<byte>> onData, Action<Exception> onException )
        {
            if ( RequestChunks.Count > 0 )
            {
                onData( RequestChunks.Dequeue() );
            }
            else
            {
                OnData = onData;
                OnException = onException;
            }
        }

        public Request()
        {
            Parameters = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            Items = new Dictionary<string, object>();
            RequestChunks = new Queue<ArraySegment<byte>>();
            OnData = Initialize;
        }

        private void Initialize( ArraySegment<byte> arraySegment )
        {
            Initialized = true;
            OnData = CacheBytes;
            RequestParser.PopulateRequest( this, arraySegment );
        }
    }
}