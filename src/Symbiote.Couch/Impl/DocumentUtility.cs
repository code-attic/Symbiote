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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Collections;
using Symbiote.Core.Reflection;
using Symbiote.Core.Utility;
using Symbiote.Couch.Config;
using Symbiote.Couch.Impl.Model;

namespace Symbiote.Couch.Impl
{
    public class DocumentUtility
    {
        protected ICouchConfiguration configuration { get; set; }
        protected IKeyAccessor KeyAccessor { get; set; }
        protected MruDictionary<string, DocumentMetadata> MetadataStore { get; set; }

        public virtual string GetDocumentIdAsJson( object instance )
        {
            var doc = instance as IHaveDocumentId;
            if ( doc != null )
            {
                return doc.GetDocumentIdAsJson();
            }
            else
            {
                return KeyAccessor.GetId( instance );
            }
        }

        public virtual object GetDocumentId( object instance )
        {
            var doc = instance as IHaveDocumentId;
            if ( doc != null )
            {
                return doc.GetDocumentId();
            }
            else
            {
                return KeyAccessor.GetId( instance );
            }
        }

        public virtual string GetDocumentRevision( object instance )
        {
            var doc = instance as IHaveDocumentRevision;
            if ( doc != null )
            {
                return doc.DocumentRevision;
            }
            else
            {
                //var documentRevision =
                //    Reflector.ReadMember( instance, configuration.Conventions.RevisionPropertyName ).ToString();
                //return string.IsNullOrEmpty( documentRevision ) ? null : documentRevision;
                return null;
            }
        }

        public virtual void SetDocumentRevision( string revision, object instance )
        {
            var doc = instance as IHaveDocumentRevision;
            if ( doc != null )
            {
                doc.UpdateRevFromJson( revision );
            }
            else
            {
                //Reflector.WriteMember( instance, configuration.Conventions.RevisionPropertyName, revision );
            }
        }

        public virtual bool IsDocument( object instance )
        {
            return instance.GetType().GetInterface( "ICouchDocument`1" ) != null;
        }

        public DocumentUtility( ICouchConfiguration couchConfiguration, IKeyAccessor keyAccessor )
        {
            configuration = couchConfiguration;
            KeyAccessor = keyAccessor;
        }
    }
}