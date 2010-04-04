using System;
using Newtonsoft.Json;
using Symbiote.Core.Extensions;

namespace Symbiote.Relax
{
    [Serializable]
    [JsonObject(MemberSerialization.OptOut)]
    public abstract class CouchDocument<TModel, TKey, TRev> : BaseDocument, ICouchDocument<TKey, TRev>
        where TModel : CouchDocument<TModel, TKey, TRev>
    {
        protected TKey _documentId;
        protected TRev _documentRev;

        public static Func<TModel, TKey> GetDocumentId = x => x._documentId;
        public static Func<TModel, TRev> GetDocumentRevision = x => x._documentRev;
        public static Action<TModel, TKey> SetDocumentId = (x,k) => x._documentId = k;
        public static Action<TModel, TRev> SetDocumentRevision= (x,r) => x._documentRev = r;

        [JsonProperty(PropertyName = "_id")]
        public TKey DocumentId
        {
            get
            {
                return GetDocumentId(this as TModel);
            }
            set
            {
                SetDocumentId(this as TModel, value);
            }
        }

        [JsonProperty(PropertyName = "_rev")]
        public TRev DocumentRevision
        {
            get
            {
                return GetDocumentRevision(this as TModel);
            }
            set
            {
                SetDocumentRevision(this as TModel, value);
            }
        }

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
            var documentId = jsonKey.FromJson<TKey>();
            DocumentId = object.Equals(documentId, default(TKey)) ? "\"{0}\"".AsFormat(jsonKey).FromJson<TKey>() : documentId;
        }

        public void UpdateRevFromJson(string jsonRev)
        {
            var documentRevision = jsonRev.FromJson<TRev>();
            DocumentRevision = object.Equals(documentRevision, default(TRev)) ? "\"{0}\"".AsFormat(jsonRev).FromJson<TRev>() : documentRevision;
        }
    }
}