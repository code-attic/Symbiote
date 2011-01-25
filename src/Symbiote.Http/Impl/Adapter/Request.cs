using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Core.Impl.Futures;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter
{
    public interface IOwinAdapter
    {
        IContext Process<T>( T context );
    }

    public interface IContextTransformer
    {
        Context From<T>( T context );
    }

    public interface IConextTransformer<T> : IContextTransformer
    {
        Context From( T context );
    }

    public class OwinAdapter : IOwinAdapter
    {
        protected Dictionary<Type, IContextTransformer> Transformers { get; set; }

        public IContext Process<T>( T context )
        {
            IContextTransformer transformer = null;
            var type = typeof(T);
            if(!Transformers.TryGetValue( type, out transformer ))
            {
                throw new KeyNotFoundException( "There was no adapter for the context type {0}".AsFormat( type.FullName ) );
            }
            return transformer.From( context );
        }

        public OwinAdapter()
        {
            Transformers = new Dictionary<Type, IContextTransformer>();
        }
    }

    public class Request
        : IRequest
    {
        public string Method { get; set; }
        public string Uri { get; set; }
        public string Url { get; set; }
        public IDictionary<string, string> Parameters { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public IDictionary<string, object> Items { get; set; }

        public Future<int> Read( byte[] buffer, int offset, int count, Func<byte[], int, int, int> callback, object state )
        {
            return Future.Of( () => callback( buffer, offset, count ) ).Now();
        }
    }

    public class Response : IResponse
    {
        public IResponseProvider Provider { get; set; }
        public string Status { get; set; }
        public IDictionary<string, IEnumerable<string>> Headers { get; set; }
        public IEnumerable<object> GetBody()
        {
            return Provider.GetResponseBody();
        }

        public Response( IResponseProvider provider, string status, IDictionary<string, IEnumerable<string>> headers )
        {
            Provider = provider;
            Status = status;
            Headers = headers;
        }
    }

    public interface IResponseProvider
    {
        IEnumerable<object> GetResponseBody();
    }

    public class Context : IContext
    {
        public IRequest Request { get; set; }
        public IResponse Response { get; set; }

        public Context( IRequest request, IResponse response )
        {
            Request = request;
            Response = response;
        }
    }
}
