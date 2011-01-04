using System;
using System.Linq;
using Core.Tests.Utility.distributor;
using Machine.Specifications;
using Symbiote.Core.Hashing;
using Symbiote.Core.Hashing.Impl;

namespace Core.Tests.Utility
{
    public class when_running_standards_test
    {
        protected static HashedRedBlackTree<int, int> tree;
        private Because of = () =>
                                 {
                                     tree = new HashedRedBlackTree<int, int>();
                                     for(int i = 1; i < 13; i++)
                                     {
                                         tree.Add(i, i);
                                     }
                                     var root = tree.Root;
                                 };

        private It tree_should_have_correct_node_count = () => 12.ShouldEqual(tree.Count);
    }

    public class when_rebalancing_tree
        : with_simple_distributor_of_lists
    {
        protected static int rebalance_to = 100;

        private Because of = () =>
                                {
                                    for (int i = 0; i < listCount; i++)
                                    {
                                        try
                                        {
                                            distributor.RebalanceNodeTo( i.ToString(), rebalance_to );
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine( e );
                                        }
                                    }
                                };

        private It all_nodes_should_have_100_virtual_nodes = () => 
            distributor.Nodes.Keys.Select( x => distributor.AliasLookup[x].Count ).All( x => x == rebalance_to ).ShouldBeTrue();
    }
}