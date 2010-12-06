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
using Symbiote.Couch.Impl.Model;
using Newtonsoft.Json;

namespace Symbiote.Couch
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class CouchDocument : BaseDocument, ICouchDocument<string>
    {
        [JsonIgnore]
        protected virtual string _docId { get; set; }

        [JsonProperty(PropertyName = "_id")]
        public virtual string DocumentId
        {
            get 
            { 
                _docId = _docId ?? Guid.NewGuid().ToString();
                return _docId;
            }
            set
            {
                _docId = value;
            }
        }

        [JsonProperty(PropertyName = "_rev")]
        public virtual string DocumentRevision { get; set; }

        public virtual string GetDocumentIdAsJson()
        {
            return DocumentId.ToString();
        }

        public virtual object GetDocumentId()
        {
            return DocumentId;
        }
        
        public virtual void UpdateKeyFromJson(string jsonKey)
        {
            DocumentId = jsonKey.FromJson<string>();
        }

        public virtual void UpdateRevFromJson(string jsonRev)
        {
            DocumentRevision = jsonRev;
        }

    }
}