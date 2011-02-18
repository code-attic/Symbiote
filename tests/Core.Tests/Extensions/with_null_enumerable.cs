using System.Collections.Generic;
using Machine.Specifications;

namespace Core.Tests.Extensions
{
    public abstract class with_null_enumerable : with_permutations
    {
        protected static List<string> enumerable;
        private Establish context = () => { enumerable = null; };
    }
}