using Machine.Specifications;
using Moq;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.URI.ViewQuery
{
    public abstract class with_couch_uri
    {
        protected static Mock<CouchUri> uriMock;
        protected static CouchUri uri { get { return uriMock.Object; } }
        protected static Symbiote.Couch.Impl.Commands.ViewQuery query { get { return new Symbiote.Couch.Impl.Commands.ViewQuery(uri); } }

        private Establish context = () =>
                                        {
                                            uriMock = new Mock<CouchUri>("http", "localhost", 5984);
                                        };
    }
}
