using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class with_trees
    {
        protected static IBalancedTree<string, string> Avl;
        protected static IBalancedTree<string, string> RedBlack;
        protected static IBalancedTree<string, string> HashedAvl;
        protected static IBalancedTree<string, string> HashedRedBlack;

        private Establish context = () => 
        { 
            Avl = new AvlTree<string,string>();
            RedBlack = new RedBlackTree<string,string>();
            HashedAvl = new HashedAvlTree<string, string>();
            HashedRedBlack = new HashedRedBlackTree<string, string>();
        };
    }

    public class with_tree_operations : with_trees
    {
        public static void AddTwentyOneNodesToTree( IBalancedTree<string, string> tree )
        {
            tree.Add( "A", "1" );
            tree.Add( "B", "2" );
            tree.Add( "C", "3" );
            tree.Add( "D", "4" );
            tree.Add( "E", "5" );
            tree.Add( "F", "6" );
            tree.Add( "G", "7" );
            tree.Add( "H", "8" );
            tree.Add( "I", "9" );
            tree.Add( "J", "10" );
            tree.Add( "K", "11" );
            tree.Add( "L", "12" );
            tree.Add( "M", "13" );
            tree.Add( "N", "14" );
            tree.Add( "O", "15" );
            tree.Add( "P", "16" );
            tree.Add( "Q", "17" );
            tree.Add( "R", "18" );
            tree.Add( "S", "19" );
            tree.Add( "T", "20" );
            tree.Add( "U", "21" );
        }

        public static void RemoveThreeNodesFromTree( IBalancedTree<string, string> tree )
        {
            tree.Delete( "H" );
        }

        public static string GetNearestNodeInTree( string key, IBalancedTree<string, string> tree )
        {
            return tree.GetNearest( key );
        }
    }

    public class when_adding_nodes_to_red_black : with_tree_operations
    {
        private Because of = () => AddTwentyOneNodesToTree( RedBlack );

        private It should_have_12_nodes_or_less_on_left_branch = () => 
            ( RedBlack as RedBlackTree<string, string>).Root.Left.Count.ShouldBeLessThanOrEqualTo( 13 );
        private It should_have_12_nodes_or_less_on_right_branch = () => 
            ( RedBlack as RedBlackTree<string, string>).Root.Right.Count.ShouldBeLessThanOrEqualTo( 13 );
    }

    public class when_adding_nodes_to_avl : with_tree_operations
    {
        private Because of = () => AddTwentyOneNodesToTree( Avl );

        private It should_have_12_nodes_or_less_on_left_branch = () => 
            ( Avl as AvlTree<string, string>).Root.Left.Count.ShouldBeLessThanOrEqualTo( 13 );
        private It should_have_12_nodes_or_less_on_right_branch = () => 
            ( Avl as AvlTree<string, string>).Root.Right.Count.ShouldBeLessThanOrEqualTo( 13 );
    }
}
