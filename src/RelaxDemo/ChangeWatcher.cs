using Symbiote.Core.Extensions;
using Symbiote.Relax;

namespace RelaxDemo
{
    public class ChangeWatcher
    {
        private IDocumentRepository<TestDocument> _couch;

        public void Start()
        {
            _couch.HandleUpdates(0, Update, null);
        }

        private void Update(ChangeRecord obj)
        {
            "An update was posted to couch: \r\n\t {0}"
                .ToInfo<ChangeWatcher>(obj.Document);
        }

        public void Stop()
        {
            _couch.StopChangeStreaming();
        }

        public ChangeWatcher(IDocumentRepository<TestDocument> couch)
        {
            _couch = couch;
        }
    }
}