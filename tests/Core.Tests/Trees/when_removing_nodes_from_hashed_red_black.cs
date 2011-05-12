using System;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_removing_nodes_from_hashed_red_black : with_populated_trees
    {
        private Because of = () => RemoveNodesFromTree( HashedRedBlack );
        
        private It should_not_have_node_h = () => HashedRedBlack.Get( "H" ).ShouldBeNull();
        private It should_not_have_node_i = () => HashedRedBlack.Get( "I" ).ShouldBeNull();
        private It should_not_have_node_j = () => HashedRedBlack.Get( "J" ).ShouldBeNull();

        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( HashedRedBlack as HashedRedBlackTree<string,string> ).Root.Left.Count -
                                                        ( HashedRedBlack as HashedRedBlackTree<string,string> ).Root.Right.Count
                                                                                     ), HashedRedBlack.Count * .75 );
    }
}