using System;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_adding_nodes_to_avl : with_populated_trees
    {
        private It should_get_max_node = () => ( Avl as AvlTree<string,string> ).GetMaxLeaf().Value.ShouldEqual( "35" );
            
        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( Avl as AvlTree<string,string> ).Root.Left.Count -
                                                        ( Avl as AvlTree<string,string> ).Root.Right.Count
                                                                                     ), Avl.Count * .75 );
    }
}