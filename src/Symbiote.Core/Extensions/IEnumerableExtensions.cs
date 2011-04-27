// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Collections.Generic;
using System.Linq;

namespace Symbiote.Core.Extensions
{
    public static class IEnumerableExtenders
    {
        public static void ForEach<T>( this IEnumerable<T> enumerable, Action<T> action )
        {
            if ( enumerable == null )
                return;

            if ( action == null )
                throw new ArgumentNullException( "action" );

            foreach( var t in enumerable )
            {
                action( t );
            }
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
                    .Select( x => x ).ToList();
        }
    }
}