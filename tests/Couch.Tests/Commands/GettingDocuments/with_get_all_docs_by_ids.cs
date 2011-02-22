using Machine.Specifications;
using Symbiote.Core.Serialization;
using Symbiote.Couch.Impl.Commands;
using Symbiote.Couch.Impl.Http;
using Symbiote.Couch.Impl.Json;

namespace Couch.Tests.Commands.GettingDocuments
{
    public abstract class with_get_all_docs_by_ids : with_command_factory
    {
        protected static KeyList keyList;
        protected static string response;
        protected static string url;
        protected static GetDocumentsByIdsCommand command;

        private Establish context = () =>
                                        {
                                            keyList = new KeyList() { keys = new object[] { "1", "2" } };
                                            response = @"{""total_rows"":""2"",""offset"":""0"",""rows"":[{""doc"":{""$type"":""Couch.Tests.Commands.TestDoc,Couch.Tests"",""_id"":""1"",""_rev"":""1"",""Message"":""Test1""}},{""doc"":{""$type"":""Couch.Tests.Commands.TestDoc,Couch.Tests"",""_id"":""2"",""_rev"":""1"",""Message"":""Test2""}}]}";
                                            url = @"http://localhost:5984/symbiotecouch/_all_docs?include_docs=true";
                                            mockAction
                                                .Setup(x => x.Post(Moq.It.Is<CouchUri>(i => i.ToString() == url), keyList.ToJson(false)))
                                                .Returns(response)
                                                .AtMostOnce();
                                            command = factory.CreateGetDocumentsByIdsCommand();
                                        };
    }
}