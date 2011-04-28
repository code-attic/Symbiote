using System;
using Symbiote.Core.Trees;

namespace Core.Tests.Trees
{
    public static class RedBlackTreeExtension
    {
        public static int Validate( this IRedBlackLeaf<string,string> leaf )
        {
            return ValidateTree( leaf );
        }

        public static int ValidateTree( IRedBlackLeaf<string,string> leaf )
        {
            int lh, rh;
            if( leaf == null )
                return 1;
            else
            {
                var left = leaf.Left;
                var right = leaf.Right;

                if( leaf.IsRed() && ( left.IsRed() || right.IsRed() ) )
                    throw new Exception("Red violation!");

                lh = ValidateTree( left );
                rh = ValidateTree( right );

                if( ( left != null && ( left.GreaterThan( leaf ) || leaf.Key.Equals( left.Key ) ) ) ||
                    ( right != null && ( right.LessThan( leaf ) || leaf.Key.Equals( right.Key ) ) ) )
                {
                    throw new Exception("Invalid binary tree structure!");
                }

                if( lh != 0 && rh != 0 && lh != rh )
                    throw new Exception("Black violation!");

                if( lh != 0 && rh != 0 )
                    return leaf.IsRed() ? lh : lh + 1;
                else
                {
                    return 0;
                }
            }
        }
    }
}