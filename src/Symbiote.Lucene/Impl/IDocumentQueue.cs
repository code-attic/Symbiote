using System.Collections.Generic;
using System.Text;
using Lucene.Net.Documents;

namespace Symbiote.Lucene.Impl
{
    public interface IDocumentQueue
    {
        void PushDocument(Document document);
    }
}
