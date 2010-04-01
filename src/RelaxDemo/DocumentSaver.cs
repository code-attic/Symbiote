using Symbiote.Relax;

namespace RelaxDemo
{
    public class DocumentSaver
    {
        private IDocumentRepository<TestDocument> _couch;

        public void Save(TestDocument document)
        {
            _couch.Save(document);
        }

        public DocumentSaver(IDocumentRepository<TestDocument> couch)
        {
            _couch = couch;
        }
    }
}