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
}
