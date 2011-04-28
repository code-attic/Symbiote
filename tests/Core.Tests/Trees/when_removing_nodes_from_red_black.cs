using System;
    using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_removing_nodes_from_red_black : with_populated_trees
    {
        private Because of = () => RemoveNodesFromTree( RedBlack );
        
        private It should_not_have_node_h = () => RedBlack.Get( "H" ).ShouldBeNull();
        private It should_not_have_node_i = () => RedBlack.Get( "I" ).ShouldBeNull();
        private It should_not_have_node_j = () => RedBlack.Get( "J" ).ShouldBeNull();

        private It should_correcty_get_nearest_node = () => RedBlack.GetNearest( "I" ).ShouldEqual( "7" );
        
        private It should_be_valid = () => ( RedBlack as RedBlackTree<string,string> ).Root.Validate().ShouldNotEqual( 0 );

        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( RedBlack as RedBlackTree<string,string> ).Root.Left.Count -
                                                        ( RedBlack as RedBlackTree<string,string> ).Root.Right.Count
                                                                                     ), RedBlack.Count * .75 );
    }
}