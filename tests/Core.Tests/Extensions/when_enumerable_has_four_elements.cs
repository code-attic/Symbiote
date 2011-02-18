using System.Linq;
using Machine.Specifications;
using Symbiote.Core.Extensions;

namespace Core.Tests.Extensions
{
    [Subject("enumerable")]
    public class when_enumerable_has_four_elements : with_four_elements
    {
        private Because of = () => { permutations = enumerable.UniquePermutations(); };

        private It should_have_fifteen_permutations = () => ShouldExtensionMethods.ShouldEqual( permutations.Count(), 15);
        private It should_have_correct_permutations = () => permutations.ShouldContain(
            new[] {
                      new [] { "A" },
                      new [] { "B" },
                      new [] { "C" },
                      new [] { "D" },
                      new [] { "A", "B"},
                      new [] { "A", "C"},
                      new [] { "A", "D"},
                      new [] { "B", "C"},
                      new [] { "B", "D"},
                      new [] { "C", "D"},
                      new [] { "A", "B", "C"},
                      new [] { "A", "B", "D"},
                      new [] { "B", "C", "D"},
                      new [] { "A", "B", "C", "D"},
                  });
    }
}