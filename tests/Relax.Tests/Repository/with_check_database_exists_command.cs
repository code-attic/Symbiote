using Machine.Specifications;
using Symbiote.Relax.Impl;

namespace Relax.Tests.Repository
{
    public abstract class with_check_database_exists_command : with_document_repository
    {
        private Establish context = () =>
                                        {
                                            uri = new CouchURI("http", "localhost", 5984, "testdocument");
                                            commandMock.Setup(x => x.Get(couchUri)).Returns("true");
                                            WireUpCommandMock(commandMock.Object);
                                        };
    }
}