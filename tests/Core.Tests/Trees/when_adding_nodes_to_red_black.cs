using System;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_adding_nodes_to_red_black : with_populated_trees
    {
        private It should_be_valid = () => ( RedBlack as RedBlackTree<string,string> ).Root.Validate().ShouldNotEqual( 0 );

        private It should_get_max_node = () => ( RedBlack as RedBlackTree<string,string> ).GetMaxLeaf().Value.ShouldEqual( "35" );

        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( RedBlack as RedBlackTree<string,string> ).Root.Left.Count -
                                                        ( RedBlack as RedBlackTree<string,string> ).Root.Right.Count
                                                                                     ), RedBlack.Count * .75 );
    }
}