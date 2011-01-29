using System;
using Machine.Specifications;
using System.Linq;
using Symbiote.Core.Trees;

namespace Core.Tests.Utility
{
    public class when_loading_tree
        : with_value_list
    {
        protected static HashedRedBlackTree<string, string> tree;
        private Because of = () =>
                                 {
                                     tree = new HashedRedBlackTree<string, string>();
                                     wordList.ForEach(w => tree.Add(w, w));
                                     var r = tree.Root;
                                 };
        
        private It tree_should_have_correct_node_count = () => wordList.Count.ShouldEqual(tree.Count);
        private It all_nodes_should_be_found_by_key = () => wordList.All(w => tree.Get(w) == w).ShouldBeTrue();
    }
}