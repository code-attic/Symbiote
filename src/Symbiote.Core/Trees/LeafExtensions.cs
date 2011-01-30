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
namespace Symbiote.Core.Trees
{
    public static class LeafExtensions
    {
        public static bool IsEmpty<TKey, TValue>( this IRedBlackLeaf<TKey, TValue> leaf )
        {
            return leaf == null; // || leaf.Value.Equals(default(TValue));
        }

        public static bool IsEmpty<TKey, TValue>( this IAvlLeaf<TKey, TValue> leaf )
        {
            return leaf == null || leaf.Value.Equals( default(TValue) );
        }

        public static bool IsRed<TKey, TValue>( this IRedBlackLeaf<TKey, TValue> leaf )
        {
            return !leaf.IsEmpty() && leaf.Color == LeafColor.RED;
        }

        public static bool IsBlack<TKey, TValue>( this IRedBlackLeaf<TKey, TValue> leaf )
        {
            return !leaf.IsEmpty() && leaf.Color == LeafColor.BLACK;
        }
    }
}