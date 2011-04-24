using Machine.Specifications;
using Moq;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Repository
{
    public abstract class with_get_documents_by_range : with_document_repository
    {
        protected static Mock<IHttpAction> commandMock;
        private Establish context = () =>
                                        {
                                            commandMock = new Mock<IHttpAction>();
                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch")
                                                .ListAll()
                                                .IncludeDocuments()
                                                .StartKey("1")
                                                .EndKey("2");

                                            commandMock.Setup(x => x.Get(couchUri))
                                                .Returns(
                                                    "{ offset: \"0\", total_rows: \"2\", rows : [ { id : \"1\", key : \"1\", doc : {_id : \"1\", _rev : \"2\", Message : \"Hello\" } } ] }");

                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}