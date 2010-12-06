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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Core.Reflection;
using Symbiote.Core.Utility;
using Symbiote.Couch.Config;
using Symbiote.Couch.Impl.Model;

namespace Symbiote.Couch.Impl
{
    public class DocumentUtility
    {
        protected ICouchConfiguration configuration { get; set; }

        public virtual string GetDocumentIdAsJson(object instance)
        {
            var doc = instance as IHaveDocumentId;
            if (doc != null)
            {
                return doc.GetDocumentIdAsJson();
            }
            else
            {
                return JsonExtensions.ToJson<object>(Reflector.ReadMember(instance, configuration.Conventions.IdPropertyName), false);
            }
        }

        public virtual object GetDocumentId(object instance)
        {
            var doc = instance as IHaveDocumentId;
            if (doc != null)
            {
                return doc.GetDocumentId();
            }
            else
            {
                return Reflector.ReadMember(instance, configuration.Conventions.IdPropertyName);
            }
        }

        public virtual string GetDocumentRevision(object instance)
        {
            var doc = instance as IHaveDocumentRevision;
            if (doc != null)
            {
                return doc.DocumentRevision;
            }
            else
            {
                var documentRevision = Reflector.ReadMember(instance, configuration.Conventions.RevisionPropertyName).ToString();
                return string.IsNullOrEmpty(documentRevision) ? null : documentRevision;
            }
        }

        public virtual void SetDocumentRevision(string revision, object instance)
        {
            var doc = instance as IHaveDocumentRevision;
            if (doc != null)
            {
                doc.UpdateRevFromJson(revision);
            }
            else
            {
                Reflector.WriteMember(instance, configuration.Conventions.RevisionPropertyName, revision);
            }
        }

        public virtual object[] GetDocumentGraph(object model)
        {
            object[] documentArray = new object[] { };
            List<object> original = new List<object>();
            if (model as IEnumerable != null)
            {
                original.AddRange((model as IEnumerable).Cast<object>());
            }
            else
            {
                original.Add(model);
            }

            var watcher = new DocumentHierarchyWatcher();
            var visitor = new HierarchyVisitor(false, IsDocument);
            visitor.Subscribe(watcher);
            visitor.Visit(model);
            documentArray = watcher.Documents.ToArray();

            return documentArray.Length == 0 ? original.ToArray() : documentArray;
        }

        public virtual bool IsDocument(object instance)
        {
            return instance.GetType().GetInterface("ICouchDocument`1") != null;
        }

        public DocumentUtility(ICouchConfiguration couchConfiguration)
        {
            this.configuration = couchConfiguration;
        }
    }
}