using System;

namespace Symbiote.Relax.Impl
{
    public class SimpleDocumentRepository :
        BaseDocumentRepository<JsonDocument, Guid, string>,
        IDocumentRepository<JsonDocument>,
        IDisposable
    {
        public SimpleDocumentRepository(IDocumentRepository<Guid, string> repository)
            : base(repository)
        {
            
        }
    }
}