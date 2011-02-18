using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    [Subject("enumerable")]
    public class when_enumerable_has_two_elements : with_two_elements
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_three_permutations = () => ShouldExtensionMethods.ShouldEqual( permutations.Count(), 3);
        private It should_have_correct_permutations = () => permutations.ShouldContain(
            new [] {
                       new [] { "A" },
                       new [] { "A", "B"},
                       new [] { "B"},
                   });
    }
}