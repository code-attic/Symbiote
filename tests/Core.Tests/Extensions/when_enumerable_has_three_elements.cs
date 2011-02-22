using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    [Subject("enumerable")]
    public class when_enumerable_has_three_elements : with_three_elements
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_seven_permutations = () => ShouldExtensionMethods.ShouldEqual( permutations.Count(), 7);
        private It should_have_correct_permutations = () => permutations.ShouldContain(
            new[] {
                      new [] { "A" },
                      new [] { "B" },
                      new [] { "C" },
                      new [] { "A", "B"},
                      new [] { "A", "C"},
                      new [] { "B", "C"},
                      new [] { "A", "B", "C"},
                  });
    }
}