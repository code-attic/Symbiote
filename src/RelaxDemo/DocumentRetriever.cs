using Symbiote.Relax;

namespace RelaxDemo
{
    public class DocumentRetriever
    {
        private IDocumentRepository<TestDocument> _couch;

        public TestDocument GetById(string id)
        {
            return _couch.Get(id);
        }

        public DocumentRetriever(IDocumentRepository<TestDocument> couch)
        {
            _couch = couch;
        }
    }
}