using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests
{
    public class when_adding_attachment_stub
    {
        protected static string test = @"{""_id"":""attachment_doc"",""_attachments"":{""foo.txt"":{""content_type"":""text/plain"",""data"":""abcdef""},""bar.txt"":{""content_type"":""text/plain"",""data"":""abcdef""}}}";

        protected static TestTarget target;

        private Because of = () =>
                                 {
                                     target = test.FromJson<TestTarget>();
                                     target.RemoveAttachment("foo.txt");
                                     target.AddAttachment("baz.txt", "text/plain", 64);
                                 };

        private It should_have_2_attachments = () => ShouldExtensionMethods.ShouldEqual(target.Attachments.Count(), 2);

        private It should_have_bar_as_first_attachment = () => ShouldExtensionMethods.ShouldEqual(target.Attachments.First(), "bar.txt");
        private It should_have_baz_as_last_attachment = () => ShouldExtensionMethods.ShouldEqual(target.Attachments.Last(), "baz.txt");
    }
}