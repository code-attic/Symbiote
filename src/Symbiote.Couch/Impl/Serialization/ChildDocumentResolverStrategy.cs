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
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;
using Symbiote.Couch.Config;
using Newtonsoft.Json.Serialization;

namespace Symbiote.Couch.Impl.Serialization
{
    public class ChildDocumentResolverStrategy : IContractResolverStrategy
    {
        protected ICouchConfiguration configuration { get; set; }

        public bool ResolverAppliesForSerialization(Type type)
        {
            return true;
        }

        public bool ResolverAppliesForDeserialization(Type type)
        {
            return false;
        }

        public IContractResolver Resolver
        {
            get { return new ChildDocumentContractResolver(configuration); }
        }

        public ChildDocumentResolverStrategy(ICouchConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}
