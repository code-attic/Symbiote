using Machine.Specifications;
using Moq;
using Symbiote.Relax;
using Symbiote.Relax.Impl;

namespace Relax.Tests.Repository
{
    public abstract class with_document_repository : with_configuration
    {
        protected static IDocumentRepository repository;
        protected static CouchURI uri;
        protected static Mock<ICouchCommand> commandMock;
        protected static CouchURI couchUri 
        {
            get
            {
                return Moq.It.Is<CouchURI>(u => u.ToString().Equals(uri.ToString()));
            }
        }
        
        private Establish context = () =>
                                        {
                                            commandMock = new Mock<ICouchCommand>();
                                            repository = new DocumentRepository(configuration, new CouchCommandFactory());
                                        };
    }
}