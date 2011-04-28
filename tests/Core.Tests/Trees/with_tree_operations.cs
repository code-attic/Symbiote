using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class with_tree_operations : with_trees
    {
        public static void AddNodesToTree( IBalancedTree<string, string> tree )
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
            tree.Add( "V", "22" );
            tree.Add( "W", "23" );
            tree.Add( "X", "24" );
            tree.Add( "Y", "25" );
            tree.Add( "Z", "26" );
            tree.Add( "ZA", "27" );
            tree.Add( "ZB", "28" );
            tree.Add( "ZC", "29" );
            tree.Add( "ZD", "30" );
            tree.Add( "ZE", "31" );
            tree.Add( "ZF", "32" );
            tree.Add( "ZG", "33" );
            tree.Add( "ZH", "34" );
            tree.Add( "ZI", "35" );
        }

        public static void RemoveNodesFromTree( IBalancedTree<string, string> tree )
        {
            tree.Delete( "H" );
            tree.Delete( "I" );
            tree.Delete( "J" );
        }
    }
}