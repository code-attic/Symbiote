/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

namespace Symbiote.Core.Hashing.Impl
{
    public class HashedRedBlackTree<TKey, TValue>
        : RedBlackTreeBase<TKey, TValue>, ITree<TKey, TValue>
    {
        protected IHashingProvider HashProvider { get; set; }

        protected override IRedBlackLeaf<TKey, TValue> CreateLeaf(TKey key, TValue value)
        {
            var newLeaf = new HashedRedBlackLeaf<TKey, TValue>()
                        {
                            HashKey = HashProvider.Hash(key),
                            Key = key,
                            Value = value,
                        };

            return newLeaf;
        }

        protected override IRedBlackLeaf<TKey, TValue> Insert(IRedBlackLeaf<TKey, TValue> root, IRedBlackLeaf<TKey, TValue> leaf)
        {
            if(Root.IsEmpty())
            {
                Root = leaf;
            }
            else
            {
                IRedBlackLeaf<TKey, TValue> g, t, p, q, head;
                head = new RedBlackLeaf<TKey, TValue>(default(TKey), default(TValue), null);
                g = t = q = p = null;
                bool direction, last;
                direction = last = false;

                t = head;
                q = t.Right = Root;

                for(;;)
                {
                    if(q.IsEmpty())
                    {
                        p[direction] = q = leaf;
                        if (q.IsEmpty())
                            return Root;
                    }
                    else if(q.Left.IsRed() && q.Right.IsRed())
                    {
                        q.Color = LeafColor.RED;
                        q.Left.Color = LeafColor.BLACK;
                        q.Right.Color = LeafColor.BLACK;
                    }

                    if(q.IsRed() && p.IsRed())
                    {
                        var newDirection = t.Right.Key.Equals(g.Key);
                        var lastIteration = p[last];
                        var lastIterationKey = lastIteration == null ? default(TKey) : lastIteration.Key;
                        if (q.Key.Equals(lastIterationKey))
                        {
                            t[newDirection] = Rotate(g, !last);
                        }
                        else
                        {
                            t[newDirection] = DoubleRotate(g, !last);
                        }
                    }

                    if(q.Key.Equals(leaf.Key))
                        break;

                    last = direction;
                    direction = q.LessThan(leaf);

                    if (!g.IsEmpty())
                        t = g;
                    g = p;
                    p = q;
                    q = q[direction];
                }

                Root = head.Right;
            }

            Root.Color = LeafColor.BLACK;
            return Root;
        }

        public HashedRedBlackTree() : base()
        {
            HashProvider = new MD5HashProvider();
        }
    }
}