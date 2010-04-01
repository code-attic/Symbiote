namespace Symbiote.Relax
{
    public interface IHandleJsonDocumentId
    {
        void UpdateKeyFromJson(string jsonKey);
        string GetIdAsJson();
    }

    public interface IHandleJsonDocumentRevision
    {
        void UpdateRevFromJson(string jsonRev);
        string GetRevAsJson();
    }

    public interface ICouchDocument<TKey, TRev>
        : IHandleJsonDocumentId, IHandleJsonDocumentRevision
    {
        TKey DocumentId { get; set; }
        TRev DocumentRevision { get; set; }
    }
}