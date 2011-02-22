using Machine.Specifications;

namespace Couch.Tests.URI
{
    [Subject("Couch URI")]
    public class when_grouping_with_level : with_basic_uri
    {
        private Because of = () => uri.Group(2);

        private It should_append_group_level_2
            = () => uri.ToString().ShouldEqual(@"http://localhost:5984/symbiotecouch?group=true&group_level=2");
    }
}