using System;
using Machine.Specifications;
using Symbiote.Core.Serialization;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Json;
using It = Moq.It;

namespace Couch.Tests.Repository
{
    public abstract class with_save_model_command : with_test_document
    {
        protected static Guid id;
        protected static string json;

        private Establish context = () =>
                                        {
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

                                            commandMock.Setup(x => x.Put(couchUri, It.Is<string>(s => s.Equals(json))))
                                                .Returns(saveResponse.ToJson(false));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}