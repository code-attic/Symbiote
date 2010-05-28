using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Tests
{
    public class TestTarget
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

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
            var jsonStub = new JProperty(attachmentName, JToken.FromObject(attachment));
            attachments.Add(jsonStub);
        }

        public void RemoveAttachment(string attachmentName)
        {
            attachments.Remove(attachmentName);
        }
    }
}