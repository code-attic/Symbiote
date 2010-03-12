using System;
using Newtonsoft.Json;

namespace Symbiote.Relax
{
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class JsonDocument : DefaultCouchDocument
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Body { get; set; }
    }
}