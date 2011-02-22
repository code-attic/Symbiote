using System.Collections.Generic;

namespace Core.Tests.Extensions
{
    public abstract class with_permutations : with_exception
    {
        protected static IEnumerable<IEnumerable<string>> permutations;
    }
}