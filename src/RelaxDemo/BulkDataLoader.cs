using System.Collections.Generic;
using Symbiote.Relax;

namespace RelaxDemo
{
    public class BulkDataLoader
    {
        private IDocumentRepository _couch;

        public IList<TestDocument> GetAllDocuments()
        {
            return _couch.GetAll<TestDocument>();
        }

        public BulkDataLoader(IDocumentRepository couch)
        {
            _couch = couch;
        }
    }
}