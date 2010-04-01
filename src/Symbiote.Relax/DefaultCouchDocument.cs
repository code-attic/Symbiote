using System;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;

namespace Symbiote.Relax
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class DefaultCouchDocument : ICouchDocument<string, string>
    {
        [JsonProperty(PropertyName = "_id")]
        public string DocumentId { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string DocumentRevision { get; set; }

        public string GetIdAsJson()
        {
            return DocumentId.ToJson(false);
        }
        public string GetRevAsJson()
        {
            return DocumentRevision.ToJson(false);
        }
        public void UpdateKeyFromJson(string jsonKey)
        {
            DocumentId = jsonKey.FromJson<string>();
        }
        public void UpdateRevFromJson(string jsonRev)
        {
            DocumentRevision = jsonRev.FromJson<string>();
        }

    }
}