using System;
using System.Text;
using Machine.Specifications;
using Moq;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Repository
{
    public abstract class with_get_attachment : with_document_repository
    {
        protected static Tuple<string, byte[]> attachment;
        protected static string content;
        protected static Mock<IHttpAction> commandMock;

        private Establish context = () =>
                                        {
                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch")
                                                .Id("1")
                                                .Attachment("myattachment");

                                            content = "This is some content. Huzzah.";
                                            var bytes = Encoding.UTF8.GetBytes(content);
                                            attachment = Tuple.Create("myattachment", bytes);
                                            commandMock = new Mock<IHttpAction>();
                                            commandMock.Setup(x => x.GetAttachment(couchUri))
                                                .Returns(attachment);

                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}