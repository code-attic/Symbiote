using Symbiote.Relax;

namespace RelaxDemo
{
    public class DatabaseDeleter
    {
        private IDocumentRepository<TestDocument> _couch;

        public void Nuke()
        {
            _couch.DeleteDatabase();
        }

        public DatabaseDeleter(IDocumentRepository<TestDocument> couch)
        {
            _couch = couch;
        }
    }
}