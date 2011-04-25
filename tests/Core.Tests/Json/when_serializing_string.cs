using Machine.Specifications;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;

namespace Core.Tests.Json
{
    [Subject("JSON Serialization")]
    public class when_serializing_string : with_assimilation
    {
        protected static string test = "this is a string";
        protected static string result;

        private Because of = () => { result = test.ToJson(false); };

        private It should_not_include_braces = () => result.ShouldEqual("\"{0}\"".AsFormat(test));
    }
}
