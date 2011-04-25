using System;
using Machine.Specifications;
using Moq;
using Symbiote.Core;
using Symbiote.Core.Serialization;
using Symbiote.Couch;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Json;
using Symbiote.Couch.Impl.Repository;
using It = Moq.It;

namespace Couch.Tests.Repository
{
    public abstract class with_save_model_command : with_test_document
    {
        protected static Guid id;
        protected static string json;
        protected static Mock<IHttpAction> commandMock;
        protected static CouchUri uri;
        protected static IDocumentRepository repository;
        protected static CouchUri couchUri 
        {
            get
            {
                return Moq.It.Is<CouchUri>(u => u.ToString().Equals(uri.ToString()));
            }
        }

        private Establish context = () =>
                                        {
                                            repository = Assimilate.GetInstanceOf<DocumentRepository>();
                                            id = Guid.NewGuid();
                                            document = new TestDocument()
                                            {
                                                DocumentId = id,
                                                Message = "Hello",
                                                DocumentRevision = "2"
                                            };
                                            json = document.ToJson();

                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch").Id(id);
                                            var saveResponse = new SaveResponse() {Id = id.ToString(), Revision = "3", Success = true};

                                            commandMock = new Mock<IHttpAction>();
                                            commandMock.Setup(x => x.Put( couchUri, It.Is<string>(s => s.Equals(json))))
                                                .Returns(saveResponse.ToJson(false));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}