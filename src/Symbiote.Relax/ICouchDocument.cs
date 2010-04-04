using System.Collections.Generic;

namespace Symbiote.Relax
{
    public interface ICouchDocument<TKey, TRev>
        : IHandleJsonDocumentId, IHandleJsonDocumentRevision
    {
        TKey DocumentId { get; set; }
        TRev DocumentRevision { get; set; }
        IEnumerable<string> Attachments { get; }
    }
}