using System.Collections.Generic;
using Symbiote.Core.Extensions;
using Symbiote.Daemon;
using Symbiote.Relax;
using Symbiote.Relax.Impl;

namespace RelaxDemo
{
    public class RelaxDemoService : IDaemon
    {
        private ICouchServer _couch;
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

            list.ForEach(x => _couch.Repository.DeleteDocument<TestDocument>(x.DocumentId, x.DocumentRevision));

            "... Done"
                .ToInfo<RelaxDemoService>();

            // adding 10 new documents one by one
            "Adding 10 new documents one at a time ..."
                .ToInfo<RelaxDemoService>();

            list = new List<TestDocument>
                           {
                               new TestDocument("Message 1"),
                               new TestDocument("Message 2"),
                               new TestDocument("Message 3"),
                               new TestDocument("Message 4"),
                               new TestDocument("Message 5"),
                               new TestDocument("Message 6"),
                               new TestDocument("Message 7"),
                               new TestDocument("Message 8"),
                               new TestDocument("Message 9"),
                               new TestDocument("Message 10"),
                           };
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

            // one time replication
            "Copying database via one-time replication ..."
                .ToInfo<RelaxDemoService>();

            _couch.CopyDatabase<TestDocument>(CouchUri.Build("http", "localhost", 5984, "copytest"));
        }

        public void Stop()
        {
            "Stopping service..."
                .ToInfo<RelaxDemoService>();
            _watcher.Stop();
            _databaseDeleter.Nuke();
        }

        public RelaxDemoService(ICouchServer couch)
        {
            _couch = couch;
            _databaseDeleter = new DatabaseDeleter(couch);
            _bulkPersister = new BulkDataPersister(couch.Repository);
            _documentSaver = new DocumentSaver(couch.Repository);
            _retriever = new DocumentRetriever(couch.Repository);
            _pager = new PagingDataLoader(couch.Repository);
            _bulkLoader = new BulkDataLoader(couch.Repository);
            _watcher = new ChangeWatcher(couch.Repository);
        }
    }
}