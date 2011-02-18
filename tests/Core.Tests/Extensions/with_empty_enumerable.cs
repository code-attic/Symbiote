using System.Collections.Generic;
using Machine.Specifications;

namespace Core.Tests.Extensions
{
    public abstract class with_empty_enumerable : with_permutations
    {
        protected static List<string> enumerable;
        protected Establish context = () => { enumerable = new List<string>(); };
    }
}