using Machine.Specifications;
using Symbiote.Couch.Impl.Commands;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Commands.GettingDocuments
{
    public abstract class with_get_all_docs : with_command_factory
    {
        protected static string response;
        protected static string url;
        protected static GetAllDocumentsCommand command;

        private Establish context = () =>
                                        {
                                            url = @"http://localhost:5984/symbiotecouch/_all_docs?include_docs=true";
                                            response = @"{""total_rows"":""2"",""offset"":""0"",""rows"":[{""doc"":{""$type"":""Couch.Tests.Commands.TestDoc,Couch.Tests"",""_id"":""1"",""_rev"":""1"",""Message"":""Test1""}},{""doc"":{""$type"":""Couch.Tests.Commands.TestDoc,Couch.Tests"",""_id"":""2"",""_rev"":""1"",""Message"":""Test2""}}]}";
                                            mockAction
                                                .Setup(x => x.Get(Moq.It.Is<CouchUri>(i => i.ToString() == url)))
                                                .Returns(response)
                                                .AtMostOnce();
                                            command = factory.CreateGetAllDocumentsCommand();
                                        };
    }

    public abstract class with_get_docs_paged : with_command_factory
    {
        protected static string response;
        protected static string url;
        protected static GetDocumentsPagedCommand command;

        private Establish context = () =>
        {
            url = @"http://localhost:5984/symbiotecouch/_all_docs?include_docs=true&skip=4&limit=2";
            response = @"{""total_rows"":""2"",""offset"":""0"",""rows"":[{""doc"":{""$type"":""Couch.Tests.Commands.TestDoc,Couch.Tests"",""_id"":""1"",""_rev"":""1"",""Message"":""Test1""}},{""doc"":{""$type"":""Couch.Tests.Commands.TestDoc,Couch.Tests"",""_id"":""2"",""_rev"":""1"",""Message"":""Test2""}}]}";
            mockAction
                .Setup(x => x.Get(Moq.It.Is<CouchUri>(i => i.ToString() == url)))
                .Returns(response)
                .AtMostOnce();
            command = factory.CreateGetDocumentsPagedCommand();
        };
    }
}