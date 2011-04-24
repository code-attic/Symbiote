using System;
using Machine.Specifications;
using Moq;
using Symbiote.Core.Serialization;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Json;
using It = Moq.It;

namespace Couch.Tests.Repository
{
    public abstract class with_save_models_command : with_test_document
    {
        protected static Guid id;
        protected static string originalDocument;
        protected static BulkPersist bulkSave;
        protected static string bulkSaveJson;
        protected static Mock<IHttpAction> commandMock;

        private Establish context = () =>
                                        {
                                            id = Guid.NewGuid();
                                            document = new TestDocument()
                                                           {
                                                               DocumentId = id,
                                                               Message = "Hello",
                                                               DocumentRevision = "2"
                                                           };
                                            originalDocument = document.ToJson(false);
                                            bulkSave = new BulkPersist(true, false, new[] {document});
                                            bulkSaveJson = bulkSave.ToString();
                                            
                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch").BulkInsert();
                                            var saveResponse = 
                                                new []
                                                    {
                                                        new SaveResponse() {Id = id.ToString(), Revision = "3", Success = true}
                                                    };
                                            commandMock = new Mock<IHttpAction>();
                                            commandMock.Setup(x => x.Post(couchUri, It.Is<string>(s => s.Equals(bulkSaveJson))))
                                                .Returns(saveResponse.ToJson(false));
                                            WireUpCommandMock(commandMock.Object);
                                        };

        protected static bool BulkPersistMatch(BulkPersist p)
        {
            var o = p.Documents[0] as TestDocument;
            return  o.DocumentId == id &&
                    o.Message == "Hello" &&
                    o.DocumentRevision == "2";
        }
    }
}