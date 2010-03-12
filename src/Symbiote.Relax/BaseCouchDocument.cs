using System;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;

namespace Symbiote.Relax
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class BaseCouchDocument<TKey, TRev> : ICouchDocument<TKey, TRev>
    {
        private TKey _id;

        [JsonProperty(PropertyName = "_id")]
        public TKey Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public TRev Revision { get; set; }

        public virtual void UpdateRevision(string revision)
        {
            Revision = revision.ConvertTo<TRev>();
        }

        public virtual void UpdateId(string id)
        {
            Id = id.ConvertTo<TKey>();
        }
    }
}