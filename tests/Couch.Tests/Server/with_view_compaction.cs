using Machine.Specifications;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Server
{
    public abstract class with_view_compaction : with_couch_server
    {
        private Establish context = () =>
                                        {
                                            uri = new CouchUri("http", "localhost", 5984, "symbiotecouch")
                                                .CompactView("testView");
                                            commandMock.Setup(x => x.Post(couchUri));
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}