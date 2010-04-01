using System.Collections.Generic;
using Symbiote.Relax;

namespace RelaxDemo
{
    public class PagingDataLoader
    {
        private IDocumentRepository<TestDocument> _couch;
        private int _page = 0;

        public IList<TestDocument> GetNext3Documents()
        {
            return _couch.GetAll(3, ++_page);
        }

        public PagingDataLoader(IDocumentRepository<TestDocument> couch)
        {
            _couch = couch;
        }
    }
}