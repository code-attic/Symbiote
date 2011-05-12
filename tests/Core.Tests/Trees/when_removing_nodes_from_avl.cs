using System;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_removing_nodes_from_avl : with_populated_trees
    {
        private Because of = () => RemoveNodesFromTree( Avl );
        
        private It should_not_have_node_h = () => Avl.Get( "H" ).ShouldBeNull();
        private It should_not_have_node_i = () => Avl.Get( "I" ).ShouldBeNull();
        private It should_not_have_node_j = () => Avl.Get( "J" ).ShouldBeNull();

        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( Avl as AvlTree<string,string> ).Root.Left.Count -
                                                        ( Avl as AvlTree<string,string> ).Root.Right.Count
                                                                                     ), Avl.Count * .75 );
    }
}