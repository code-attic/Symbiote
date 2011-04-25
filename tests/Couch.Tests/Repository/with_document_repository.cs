using Machine.Specifications;
using Moq;
using Symbiote.Core;
using Symbiote.Couch;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Repository;

namespace Couch.Tests.Repository
{
    //public abstract class with_document_repository : with_configuration
    //{
    //    protected static IDocumentRepository repository;
    //    protected static CouchUri uri;
    //    //protected static Mock<IHttpAction> commandMock;
    //    protected static CouchUri couchUri 
    //    {
    //        get
    //        {
    //            return Moq.It.Is<CouchUri>(u => u.ToString().Equals(uri.ToString()));
    //        }
    //    }
        
    //    private Establish context = () =>
    //                                    {
    //                                        //commandMock = new Mock<IHttpAction>();
    //                                        repository = Assimilate.GetInstanceOf<DocumentRepository>();
    //                                    };
    //}


}