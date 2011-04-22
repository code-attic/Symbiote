using Machine.Specifications;
using Moq;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Repository
{
    public abstract class with_from_view_command : with_document_repository
    {
        protected static string jsonResult;
        protected static Mock<IHttpAction> commandMock;

        private Establish context = () =>
                                        {
                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch")
                                                .Design("test")
                                                .View("test")
                                                .NoReduce();

                                            jsonResult = @"
{
    total_rows: 2,
    offset: 0,
    rows:
        [
            {
                doc: { _id: ""test"", _rev: ""1"", Message: ""Hi"" }
            },
            {
                doc: { _id: ""_design/mydesigndoc"" }
            }
        ]
}
";
                                            commandMock = new Mock<IHttpAction>();
                                            commandMock
                                                .Setup(x => x.Get(couchUri))
                                                .Returns(jsonResult);

                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}