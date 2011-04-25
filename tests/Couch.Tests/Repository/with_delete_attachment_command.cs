using Couch.Tests.Commands;
using Machine.Specifications;
using Moq;
using Symbiote.Core;
using Symbiote.Couch;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Repository;

namespace Couch.Tests.Repository
{
    public abstract class with_delete_attachment_command : with_configuration
    {
        protected static string attachmentName;
        protected static TestDoc document;
        protected static Mock<IHttpAction> commandMock;
        protected static CouchUri uri;
        protected static IDocumentRepository repository;
        
        private Establish context = () =>
                                        {
                                            repository = Assimilate.GetInstanceOf<DocumentRepository>();
                                            document = new TestDoc() {DocumentId = "1", DocumentRevision = "1"};
                                            attachmentName = "myattachment";
                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch")
                                                .Id(document.DocumentId)
                                                .Attachment(attachmentName)
                                                .Revision(document.DocumentRevision);
                                            commandMock = new Mock<IHttpAction>();
                                            commandMock
                                                .Setup(x => x.Delete(Moq.It.IsAny<CouchUri>()));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}