using System;
using Newtonsoft.Json;

namespace Symbiote.Relax
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class DefaultCouchDocument : ICouchDocument
    {
        private Guid? _documentId;

        [JsonProperty(PropertyName = "_id")]
        public string DocumentId
        {
            get
            {
                _documentId = _documentId ?? Guid.NewGuid();
                return _documentId.ToString();
            }
            set
            {
                _documentId = new Guid(value);
            }
        }

        [JsonProperty(PropertyName = "_rev")]
        public string DocumentRevision { get; set; }
    }
}