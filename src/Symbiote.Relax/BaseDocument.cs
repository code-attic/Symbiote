using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Symbiote.Relax
{
    public abstract class BaseDocument : IHaveAttachments
    {
        [JsonProperty("_attachments")]
        private JObject attachments { get; set; }

        [JsonIgnore]
        public IEnumerable<string> Attachments
        {
            get
            {
                return attachments.Root.Children().Select(x => (x as JProperty).Name);
            }
        }

        public void AddAttachment(string attachmentName, string contentType, long contentLength)
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

        public void RemoveAttachment(string attachmentName)
        {
            if (attachments.Properties().Any(x => x.Name == attachmentName))
            {
                attachments.Remove(attachmentName);
            }
        }

        protected BaseDocument()
        {
            attachments = JObject.FromObject(new object());
        }
    }
}