using System;
using Newtonsoft.Json;

namespace Symbiote.Relax
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class DefaultCouchDocument : ICouchDocument<Guid, string>
    {
        private Guid? _id;

        [JsonProperty(PropertyName = "_id")]
        public Guid Id
        {
            get
            {
                _id = _id ?? Guid.NewGuid();
                return (Guid)_id;
            }
            set
            {
                _id = value;
            }
        }

        [JsonProperty(PropertyName = "_rev")]
        public string Revision { get; set; }

        public virtual void UpdateRevision(string revision)
        {
            Revision = revision;
        }

        public virtual void UpdateId(string id)
        {
            _id = new Guid(id);
        }
    }
}