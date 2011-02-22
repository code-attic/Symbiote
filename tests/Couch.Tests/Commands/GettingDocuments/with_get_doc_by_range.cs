using Machine.Specifications;
using Symbiote.Couch.Impl.Commands;
using Symbiote.Couch.Impl.Http;

namespace Couch.Tests.Commands.GettingDocuments
{
    public abstract class with_get_doc_by_range : with_command_factory
    {
        protected static string response;
        protected static string url;
        protected static GetDocumentsInRangeCommand command;

        private Establish context = () =>
                                        {
                                            url = @"http://localhost:5984/symbiotecouch/_all_docs?include_docs=true&startkey=""doc+1""&endkey=""doc+2""";
                                            response = @"
{ 
    total_rows: 1,
    offset: 0,
    rows: 
        [ 
            {
                doc: { 
                    $type: ""Couch.Tests.Commands.TestDoc, Couch.Tests"",
                    _id: ""1"",
                    _rev: ""1"",
                    Message: ""Test""
                }
            }
        ]
}";
                                            mockAction
                                                .Setup(x => x.Get(Moq.It.Is<CouchUri>(i => i.ToString() == url)))
                                                .Returns(response)
                                                .AtMostOnce();
                                            command = factory.CreateGetDocumentsInRangeCommand();
                                        };
    }
}