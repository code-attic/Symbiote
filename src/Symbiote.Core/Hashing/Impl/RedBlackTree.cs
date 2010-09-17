using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Core.Hashing.Impl
{
    public class RedBlackTree<TKey, TValue>
        : RedBlackTreeBase<TKey, TValue>
    {
        protected override IRedBlackLeaf<TKey, TValue> CreateLeaf(TKey key, TValue value)
        {
            return new RedBlackLeaf<TKey, TValue>(key, value, null);
        }
    }
}
