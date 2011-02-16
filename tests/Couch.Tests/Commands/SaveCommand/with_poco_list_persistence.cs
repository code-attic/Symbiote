using Machine.Specifications;
using Symbiote.Couch.Impl.Commands;
using Symbiote.Couch.Impl.Json;

namespace Couch.Tests.Commands.SaveCommand
{
    public abstract class with_poco_list_persistence : with_poco_list
    {
        protected static BulkPersist persist;
        protected static string json;
        protected static ISaveDocuments command;

        private Establish context = () =>
            {
                persist = new BulkPersist(true, false, testDocs);
                command = factory.CreateSaveDocumentsCommand();
            };
    }
}