using Machine.Specifications;
using Symbiote.Core.Serialization;
using Symbiote.Couch.Impl.Json;

namespace Couch.Tests.Serialization
{
    public class when_deserializing_view_result_and_document_excluded : with_view_result_and_no_documents
    {
        protected static ViewResult<TestDocument> result;

        private Because of = () =>
                                 {
                                     result = viewResultJson.FromJson<ViewResult<TestDocument>>();
                                 };

        private It should_have_document_message = () => 
                                                  result.Rows[0].Model.Message.ShouldEqual("Hi");
    }
}