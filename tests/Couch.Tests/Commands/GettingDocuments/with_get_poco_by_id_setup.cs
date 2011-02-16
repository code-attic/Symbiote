using Machine.Specifications;
using Symbiote.Couch.Impl.Commands;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Commands.GettingDocuments
{
    public abstract class with_get_poco_by_id_setup : with_command_factory
    {
        protected static string response;
        protected static string url;
        protected static GetDocumentCommand command;
        
        private Establish context = () =>
            {
                url = @"http://localhost:5984/symbiotecouch/1";
                response = @"{""$type"":""Couch.Tests.Commands.MyPoco,Couch.Tests"",""_id"":""1"",""_rev"":""1"",""Message"":""Test"",""MyId"":""1""}";
                mockAction
                    .Setup(x => x.Get(Moq.It.Is<CouchUri>(i => i.ToString() == url)))
                    .Returns(response)
                    .AtMostOnce();
                command = factory.CreateGetDocumentCommand();
            };
    }
}