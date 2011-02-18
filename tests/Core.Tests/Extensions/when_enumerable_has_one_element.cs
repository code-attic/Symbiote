using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    [Subject("enumerable")]
    public class when_enumerable_has_one_element : with_one_element
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_one_permutation = () => ShouldExtensionMethods.ShouldEqual( permutations.Count(), 1);
        private It should_have_correct_permutations = () => permutations.ShouldContain(new List<string>{"A"});
    }
}