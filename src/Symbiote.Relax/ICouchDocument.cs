namespace Symbiote.Relax
{
    public interface ICouchDocument
    {
        string DocumentId { get; set; }
        string DocumentRevision { get; set; }
    }
}