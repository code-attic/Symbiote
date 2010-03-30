using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Symbiote.Relax.Impl
{
    public class BulkPersist<TModel, TKey, TRev>
        where TModel : class, ICouchDocument<TKey, TRev>
    {
        [JsonProperty(PropertyName = "all_or_nothing")]
        public bool AllOrNothing { get; set; }

        [JsonProperty(PropertyName = "non_atomic")]
        public bool NonAtomic { get; set; }

        [JsonProperty(PropertyName = "docs")]
        public TModel[] Documents { get; set; }

        public BulkPersist(IEnumerable<TModel> docs)
        {
            Documents = docs.ToArray();
        }

        public BulkPersist(bool allOrNothing, bool nonAtomic, IEnumerable<TModel> docs)
        {
            AllOrNothing = allOrNothing;
            NonAtomic = nonAtomic;
            Documents = docs.ToArray();
        }
    }

    public class BulkResponse
    {
        [JsonProperty(PropertyName = "ok")]
        public bool Success { get; set; }

        [JsonProperty(PropertyName = "new_revs")]
        public NewRevision[] Revisions { get; set; }
    }

    public class NewRevision
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "rev")]
        public string Revision { get; set; }
    }
}