using Symbiote.Relax;

namespace RelaxDemo
{
    public class DocumentRetriever
    {
        private IDocumentRepository _couch;

        public TestDocument GetById(string id)
        {
            return _couch.Get<TestDocument>(id);
        }

        public DocumentRetriever(IDocumentRepository couch)
        {
            _couch = couch;
        }
    }
}