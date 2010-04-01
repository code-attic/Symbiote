using System.Collections.Generic;
using Symbiote.Relax;

namespace RelaxDemo
{
    public class BulkDataLoader
    {
        private IDocumentRepository<TestDocument> _couch;

        public IList<TestDocument> GetAllDocuments()
        {
            return _couch.GetAll();
        }

        public BulkDataLoader(IDocumentRepository<TestDocument> couch)
        {
            _couch = couch;
        }
    }
}