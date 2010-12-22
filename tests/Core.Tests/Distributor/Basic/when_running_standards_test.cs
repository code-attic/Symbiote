using Machine.Specifications;
using Symbiote.Core.Hashing;
using Symbiote.Core.Hashing.Impl;

namespace Core.Tests.Utility
{
    public class when_running_standards_test
    {
        protected static HashedRedBlackTree<int, int> tree;
        private Because of = () =>
                                 {
                                     tree = new HashedRedBlackTree<int, int>();
                                     for(int i = 1; i < 13; i++)
                                     {
                                         tree.Add(i, i);
                                     }
                                     var root = tree.Root;
                                 };

        private It tree_should_have_correct_node_count = () => 12.ShouldEqual(tree.Count);
    }
}