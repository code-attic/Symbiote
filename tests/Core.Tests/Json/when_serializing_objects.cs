using Machine.Specifications;
using Symbiote.Core.Serialization;

namespace Core.Tests.Json
{
    public class when_serializing_objects : with_assimilation
    {
        protected static TestClass test = new TestClass() { Content = "yay"};
        protected static TestClass result;

        private Because of = () =>
                                 {
                                     var json = test.ToJson();
                                     result = json.FromJson<TestClass>();
                                 };

        private It should_deserialize_correctly = () => result.Content.ShouldEqual(test.Content);
    }
}