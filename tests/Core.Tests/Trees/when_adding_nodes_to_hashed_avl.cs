using System;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_adding_nodes_to_hashed_avl : with_populated_trees
    {
        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( HashedAvl as HashedAvlTree<string,string> ).Root.Left.Count -
                                                        ( HashedAvl as HashedAvlTree<string,string> ).Root.Right.Count
                                                                                     ), HashedAvl.Count * .75 );
    }
}