using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests
{
    [Subject("JSON Serialization")]
    public class when_serializing_string
    {
        protected static string test = "this is a string";
        protected static string result;

        private Because of = () => { result = test.ToJson(false); };

        private It should_not_include_braces = () => result.ShouldEqual("\"{0}\"".AsFormat(test));
    }
}
