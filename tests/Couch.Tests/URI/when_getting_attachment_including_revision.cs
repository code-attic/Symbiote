using Machine.Specifications;

namespace Couch.Tests.URI
{
    [Subject("Couch URI")]
    public class when_getting_attachment_including_revision : with_basic_uri
    {
        private Because of = () => uri.Id("id").Attachment("foo.txt").Revision("1-A");

        private It should_append_attachment
            = () => uri.ToString().ShouldEqual(@"http://localhost:5984/symbiotecouch/id/foo.txt?rev=1-A");
    }
}