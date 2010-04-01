using Symbiote.Relax;

namespace RelaxDemo
{
    public class DatabaseDeleter
    {
        private IDocumentRepository _couch;

        public void Nuke()
        {
            _couch.DeleteDatabase<TestDocument>();
        }

        public DatabaseDeleter(IDocumentRepository couch)
        {
            _couch = couch;
        }
    }
}