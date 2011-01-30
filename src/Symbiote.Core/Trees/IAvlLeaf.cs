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
    public interface IAvlLeaf<TKey, TValue>
    {
        TKey Key { get; set; }
        IAvlLeaf<TKey, TValue> Parent { get; set; }
        TValue Value { get; set; }
        IAvlLeaf<TKey, TValue> Right { get; set; }
        IAvlLeaf<TKey, TValue> Left { get; set; }
        bool IsRoot { get; }
        int Count { get; }
        int Balance { get; set; }
        IAvlLeaf<TKey, TValue> this[ bool right ] { get; set; }
        TValue Get( TKey key );
        bool GreaterThan( TKey key );
        bool LessThan( TKey key );
        IAvlLeaf<TKey, TValue> Seek( TKey key );
    }
}