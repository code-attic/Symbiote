using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Symbiote.Core.Impl.Futures;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter
{
    public class Request
        : IRequest
    {
        public IPEndPoint ClientEndpoint { get; set; }
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Server { get; set; }
        public Stream Stream { get; set; }
        public string Uri { get; set; }
        public string Url { get; set; }
        public string Version { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public IDictionary<string, object> Items { get; set; }

        public Future<int> Read( byte[] buffer, int offset, int length, Action<int> callback, Action<Exception> onException )
        {
            return Future
                .Of( () => Stream.Read( buffer, offset, length ) )
                .OnValue( callback )
                .OnException( onException )
                .Now();
        }

        public Request()
        {
            Parameters = new Dictionary<string, string>();
            Headers = new Dictionary<string, IEnumerable<string>>();
            Items = new Dictionary<string, object>();
        }
    }
}
