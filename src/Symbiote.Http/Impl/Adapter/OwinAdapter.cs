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