using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Tests.Hashing
{
    public static class Extensions
    {
        public static MD5HashProvider _md5er;
        public static MD5HashProvider MD5er
        {
            get 
            {
                _md5er = _md5er ?? new MD5HashProvider();
                return _md5er;
            }
        }

        public static long MD5<T>( this T value )
        {
            return MD5er.Hash( value );
        }

        public static IEnumerable<IEnumerable<T>> UniquePermutations<T>( this IEnumerable<T> enumerable )
        {
            var count = enumerable.Count();
            int total = (int) (Math.Pow( 2, count )) - 1;
            var permutations = (from m in Enumerable.Range( 1, 1 << count )
                                select
                                    from i in Enumerable.Range( 0, count )
                                    where (m & (1 << i)) != 0
                                    select enumerable.Skip( i ).Take( 1 ).First());
            return
                permutations
                    .Take( total )
                    .Select( x => x );
        }
    }
}