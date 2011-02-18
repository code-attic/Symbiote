using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    [Subject("enumerable")]
    public class when_enumerable_is_empty : with_empty_enumerable
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_return_an_empty_enumeration = () => permutations.ShouldBeEmpty();
    }
}