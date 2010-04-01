using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Relax;

namespace RelaxDemo
{
    public class RelaxDemoService : IDaemon
    {
        private IDocumentRepository _couch;
        private DatabaseDeleter _databaseDeleter;
        private BulkDataPersister _bulkPersister;
        private DocumentSaver _documentSaver;
        private DocumentRetriever _retriever;
        private PagingDataLoader _pager;
        private BulkDataLoader _bulkLoader;
        private ChangeWatcher _watcher;

        public void Start()
        {
            "Starting service ..."
                .ToInfo<RelaxDemoService>();

            // start watcher
            "Starting change watcher."
                .ToInfo<RelaxDemoService>();
            _watcher.Start();

            // create bulk documents
            "Creating 10 documents via bulk insertion ..."
                .ToInfo<RelaxDemoService>();

            _bulkPersister.SaveDocuments();
            
            "... Done"
                .ToInfo<RelaxDemoService>();

            // retrieving all documents
            "Retrieve all documents at once ..."
                .ToInfo<RelaxDemoService>();

            var list = _bulkLoader.GetAllDocuments();

            "Retrieved {0} documents"
                .ToInfo<RelaxDemoService>(list.Count);

            // save each document once
            "Resaving each document (testing change updates) ..."
                .ToInfo<RelaxDemoService>();

            list.ForEach(_documentSaver.Save);

            "... Done"
                .ToInfo<RelaxDemoService>();

            // loading each doc by id
            "Loading each document by id ..."
                .ToInfo<RelaxDemoService>();

            list.ForEach(x => _retriever.GetById(x.DocumentId));

            "... Done"
                .ToInfo<RelaxDemoService>();

            // deleting each document
            "Deleting all documents ..."
                .ToInfo<RelaxDemoService>();

            list.ForEach(x => _couch.DeleteDocument<TestDocument>(x.DocumentId, x.DocumentRevision));

            "... Done"
                .ToInfo<RelaxDemoService>();

            // adding 10 new documents one by one
            "Adding 10 new documents one at a time ..."
                .ToInfo<RelaxDemoService>();

            list.ForEach(_documentSaver.Save);

            "... Done"
                .ToInfo<RelaxDemoService>();

            // getting all documents by paging
            "Testing paging by retrieving page sizes of 3 ..."
                .ToInfo<RelaxDemoService>();

            for (int i = 1; i < 5; i++)
            {
                var docs = _pager.GetNext3Documents();
                "Page {0} has {1} records"
                    .ToInfo<RelaxDemoService>(i, docs.Count);
            }
        }

        public void Stop()
        {
            "Stopping service..."
                .ToInfo<RelaxDemoService>();
            _watcher.Stop();
            _databaseDeleter.Nuke();
        }

        public RelaxDemoService(IDocumentRepository couch)
        {
            _couch = couch;
            _databaseDeleter = new DatabaseDeleter(couch);
            _bulkPersister = new BulkDataPersister(couch);
            _documentSaver = new DocumentSaver(couch);
            _retriever = new DocumentRetriever(couch);
            _pager = new PagingDataLoader(couch);
            _bulkLoader = new BulkDataLoader(couch);
            _watcher = new ChangeWatcher(couch);
        }
    }
}