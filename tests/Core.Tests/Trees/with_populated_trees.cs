using Machine.Specifications;

namespace Core.Tests.Trees
{
    public class with_populated_trees : with_tree_operations
    {
        private Establish context = () => { 
                                              AddNodesToTree( RedBlack );
                                              AddNodesToTree( HashedRedBlack );
                                              AddNodesToTree( Avl );
                                              AddNodesToTree( HashedAvl );
        };
    }
}