using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Serialization;

namespace Core.Tests.Json
{
    [Subject("JSON Deserialization")]
    public class when_deserializing_attachments : with_core_assimilation
    {
        protected static string test = @"{""_id"":""attachment_doc"",""_attachments"":{""foo.txt"":{""content_type"":""text/plain"",""data"":""abcdef""},""bar.txt"":{""content_type"":""text/plain"",""data"":""abcdef""}}}";

        protected static TestTarget target;

        private Because of = () => { target = test.FromJson<TestTarget>(); };

        private It should_not_be_null = () =>
                                            {
                                                target.ShouldNotBeNull();
                                            };

        private It should_have_2_attachments = () => ShouldExtensionMethods.ShouldEqual(target.Attachments.Count(), 2);

        private It should_have_foo_as_first_attachment = () => ShouldExtensionMethods.ShouldEqual(target.Attachments.First(), "foo.txt");
        private It should_have_bar_as_last_attachment = () => ShouldExtensionMethods.ShouldEqual(target.Attachments.Last(), "bar.txt");

        private It should_serialize_correctly = () => target.ToJson(false).Replace(@"""$id"":""1"",","").ShouldEqual(test);
    }
}