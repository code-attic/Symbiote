namespace Symbiote.Relax
{
    public interface ICouchDocument<TKey, TRev>
    {
        TKey Id { get; set; }
        TRev Revision { get; set; }

        void UpdateRevision(string revision);
        void UpdateId(string id);
    }
}