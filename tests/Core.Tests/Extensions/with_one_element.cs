using System.Collections.Generic;
using Machine.Specifications;

namespace Core.Tests.Extensions
{
    public abstract class with_one_element : with_permutations
    {
        protected static List<string> enumerable;
        private Establish context = () => { enumerable = new List<string> {"A"}; };
    }
}