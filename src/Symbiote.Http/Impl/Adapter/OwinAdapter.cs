using System;
using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter
{
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
}