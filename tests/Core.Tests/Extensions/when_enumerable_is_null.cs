using System;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    [Subject("enumerable")]
    public class when_enumerable_is_null : with_null_enumerable
    {
        private Because of = () =>
                                 {
                                     _resultingException = Catch.Exception(() => permutations = enumerable.UniquePermutations());
                                 };

        private It should_throw_argument_null_exception = () => _resultingException.ShouldBeOfType
                                                                    <ArgumentNullException>();
    }
}