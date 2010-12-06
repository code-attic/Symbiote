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

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Symbiote.Core.Extensions;

namespace Symbiote.Couch.Impl.Model
{
    public abstract class BaseDocument : IHaveAttachments
    {
        [JsonProperty("_attachments")]
        private JObject attachments { get; set; }

        [JsonProperty(PropertyName = "$doc_type")]
        internal virtual string UnderlyingDocumentType
        {
            get
            {
                return GetType().Name;
            }
            set
            {
                //do nothing, this is effectively read only in the model
            }
        }

        [JsonProperty("$doc_related_ids")]
        internal virtual Dictionary<string, object[]> RelatedDocumentIds { get; set; }

        [JsonProperty("$doc_parent_id")]
        internal virtual object ParentId { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<string> Attachments
        {
            get
            {
                return attachments.Root.Children().Select(x => (x as JProperty).Name);
            }
        }

        public virtual void AddAttachment(string attachmentName, string contentType, long contentLength)
        {
            var attachment = new
            {
                Stub = true,
                ContentType = contentType,
                ContentLength = contentLength
            };

            if (!attachments.Properties().Any(x => x.Name == attachmentName))
            {
                var jsonStub = new JProperty(attachmentName, JToken.FromObject(attachment));
                attachments.Add(jsonStub);
            }
            else
            {
                attachments.Property(attachmentName).Value = JToken.FromObject(attachment);
            }
        }

        public virtual void RemoveAttachment(string attachmentName)
        {
            if (attachments.Properties().Any(x => x.Name == attachmentName))
            {
                attachments.Remove(attachmentName);
            }
        }

        protected BaseDocument()
        {
            attachments = JObject.FromObject(new object());
            RelatedDocumentIds = new Dictionary<string, object[]>();
        }
    }
}