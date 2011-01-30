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
using Symbiote.Couch.Config;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Symbiote.Couch.Impl.Serialization
{
    public class ChildDocumentContractResolver : DefaultContractResolver
    {
        private const string ICOUCHDOC_TYPE = "ICouchDocument`1";
        private const string IENUMERABLE_TYPE = "IEnumerable`1";

        private ICouchConfiguration configuration { get; set; }

        protected override IList<JsonProperty> CreateProperties( JsonObjectContract contract )
        {
            var basePropertyList = base.CreateProperties( contract );
            return
                !configuration.BreakDownDocumentGraphs
                    ? basePropertyList
                    : basePropertyList
                          .Where( ShouldIncludeProperty )
                          .ToList();
        }

        protected bool ShouldIncludeProperty( JsonProperty property )
        {
            var propertyType = property.PropertyType;
            var interfaces = propertyType.GetInterfaces();
            if ( interfaces.Any( x => x.Name == IENUMERABLE_TYPE ) && propertyType.IsGenericType )
            {
                var paramType = propertyType.GetGenericArguments()[0];
                if ( paramType == null || paramType.GetInterface( ICOUCHDOC_TYPE ) == null )
                    return true;
            }
            else if ( propertyType.GetInterface( ICOUCHDOC_TYPE ) == null )
            {
                return true;
            }
            return false;
        }

        public ChildDocumentContractResolver( ICouchConfiguration configuration )
        {
            this.configuration = configuration;
        }

        public ChildDocumentContractResolver( bool shareCache, ICouchConfiguration configuration )
            : base( shareCache )
        {
            this.configuration = configuration;
        }
    }
}