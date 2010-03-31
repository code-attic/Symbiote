using System;
using Newtonsoft.Json;

namespace Symbiote.Relax
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class CustomCouchDocument : ICouchDocument
    {
        [JsonProperty(PropertyName = "_id")]
        public string DocumentId
        {
            get
            {
                return GetDocumentId();
            }
            set
            {
                SetDocumentId(value);
            }
        }

        [JsonProperty(PropertyName = "_rev")]
        public string DocumentRevision 
        { 
            get
            {
                return GetDocumentRevision();
            }
            set
            {
                SetDocumentRevision(value);
            }
        }

        public abstract string GetDocumentId();
        public abstract void SetDocumentId(string key);
        public abstract string GetDocumentRevision();
        public abstract void SetDocumentRevision(string key);
    }
}