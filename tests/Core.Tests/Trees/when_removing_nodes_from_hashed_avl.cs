using System;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_removing_nodes_from_hashed_avl : with_populated_trees
    {
        private Because of = () => RemoveNodesFromTree( HashedAvl );
        
        private It should_not_have_node_h = () => HashedAvl.Get( "H" ).ShouldBeNull();
        private It should_not_have_node_i = () => HashedAvl.Get( "I" ).ShouldBeNull();
        private It should_not_have_node_j = () => HashedAvl.Get( "J" ).ShouldBeNull();

        private It should_correcty_get_nearest_node = () => HashedAvl.GetNearest( "I" ).ShouldEqual( "7" );

        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( HashedAvl as HashedAvlTree<string,string> ).Root.Left.Count -
                                                        ( HashedAvl as HashedAvlTree<string,string> ).Root.Right.Count
                                                                                     ), HashedAvl.Count * .75 );
    }
}