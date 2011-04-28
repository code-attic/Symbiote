using System;
using Machine.Specifications;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public class when_adding_nodes_to_hashed_red_black : with_populated_trees
    {
        private It should_be_valid = () => ( HashedRedBlack as HashedRedBlackTree<string,string> ).Root.Validate().ShouldNotEqual( 0 );

        private It should_have_balanced = () => ShouldExtensionMethods.ShouldBeLessThanOrEqualTo( Math.Abs(
                                                        ( HashedRedBlack as HashedRedBlackTree<string,string> ).Root.Left.Count -
                                                        ( HashedRedBlack as HashedRedBlackTree<string,string> ).Root.Right.Count
                                                                                     ), HashedRedBlack.Count * .75 );
    }
}