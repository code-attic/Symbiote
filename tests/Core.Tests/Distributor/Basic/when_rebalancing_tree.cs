using System;
using System.Linq;
using Machine.Specifications;

namespace Core.Tests.Distributor.Basic
{
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
                                             LoadBalancer.RebalanceNodeTo( i.ToString(), rebalance_to );
                                         }
                                         catch (Exception e)
                                         {
                                             Console.WriteLine( e );
                                         }
                                     }
                                 };

        private It all_nodes_should_have_100_virtual_nodes = () => 
                                                             ShouldExtensionMethods.ShouldBeTrue( LoadBalancer.Nodes.Keys.Select( x => LoadBalancer.AliasLookup[x].Count ).All( x => x == rebalance_to ) );
    }
}