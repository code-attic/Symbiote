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

using System;
using Symbiote.Core.Hashing;

namespace Symbiote.Core.Trees
{
    public abstract class AvlTreeBase<TKey, TValue>
        : ITree<TKey, TValue>
    {
        public IAvlLeaf<TKey, TValue> Root { get; set; }

        public virtual int Count
        {
            get { return Root.Count; }
        }

        public virtual void Delete(TKey key)
        {
            bool done = false;
            Root = Remove(Root, key, ref done);
        }

        public virtual TValue Get(TKey key)
        {
            return Root.Get(key);
        }

        public virtual void Add(TKey key, TValue value)
        {
            var leaf = CreateLeaf(key, value);
            var done = false;
            Root = Insert(Root, leaf, ref done);
        }

        protected abstract IAvlLeaf<TKey, TValue> CreateLeaf(TKey key, TValue value);

        public virtual TValue GetNearest<T>(T key)
        {
            IAvlLeaf<TKey, TValue> leaf = Root;
            IAvlLeaf<TKey, TValue> last = null;
            var compareKey = (TKey)Convert.ChangeType(key, typeof(TKey));

            while (leaf != null)
            {
                if (leaf.GreaterThan(compareKey))
                {
                    leaf = leaf.Left;
                }
                else if (leaf.LessThan(compareKey))
                {
                    last = leaf;
                    leaf = leaf.Right;
                }
                else
                {
                    return leaf.Value;
                }
            }
            if (last.IsEmpty())
            {
                return GetMaxLeaf().Value;
            }
            else
                return last.Value;
        }

        protected IAvlLeaf<TKey, TValue> GetMaxLeaf()
        {
            IAvlLeaf<TKey, TValue> leaf;
            leaf = this.Root;
            while (leaf.Right != null)
            {
                leaf = leaf.Right;
            }
            return leaf;
        }

        protected virtual IAvlLeaf<TKey, TValue> Insert(IAvlLeaf<TKey, TValue> root, IAvlLeaf<TKey, TValue> leaf, ref bool done)
        {
            if (root.IsEmpty())
            {
                root = leaf;
            }
            else
            {
                leaf.Parent = root;
                var direction = leaf.GreaterThan(root.Key);
                root[direction] = Insert(root[direction], leaf, ref done);

                if(!done)
                {
                    root.Balance += direction ? 1 : -1;
                    if(root.Balance == 0)
                    {
                        done = true;
                    }
                    else if(Math.Abs(root.Balance) > 1)
                    {
                        root = BalancePostInsert(root, direction);
                        done = true;
                    }
                }
            }
            return root;
        }

        protected virtual IAvlLeaf<TKey, TValue> Remove(IAvlLeaf<TKey, TValue> root, TKey key, ref bool done)
        {
            if(!root.IsEmpty())
            {
                bool direction = false;

                if(root.Key.Equals(key))
                {
                    if(root.Left.IsEmpty() || root.Right.IsEmpty())
                    {
                        direction = root.Left.IsEmpty();
                        var worker = root[direction];
                        return worker;
                    }
                    else
                    {
                        var worker = root.Left;
                        while (!worker.Right.IsEmpty())
                            worker = worker.Right;

                        root.Key = worker.Key;
                        key = worker.Key;
                    }
                }

                direction = root.LessThan(key);
                root[direction] = Remove(root[direction], key, ref done);

                if(!done)
                {
                    root.Balance += direction ? -1 : 1;

                    if(Math.Abs(root.Balance) == 1)
                    {
                        done = true;
                    }
                    else if(Math.Abs(root.Balance) > 1)
                    {
                        root = BalancePostDelete(root, direction, ref done);
                    }
                }
            }
            return root;
        }

        protected virtual IAvlLeaf<TKey, TValue> BalancePostInsert(IAvlLeaf<TKey, TValue> root, bool direction)
        {
            var leaf = root[direction];
            var balance = direction ? 1 : -1;

            if(leaf.Balance == balance)
            {
                root.Balance = leaf.Balance = 0;
                root = Rotate(root, !direction);
            }
            else
            {
                DoubleRotateRebalance(root, direction, balance);
                root = DoubleRotate(root, !direction);
            }
            return root;
        }

        protected virtual IAvlLeaf<TKey, TValue> BalancePostDelete(IAvlLeaf<TKey, TValue> root, bool direction, ref bool done)
        {
            var leaf = root[!direction];
            var balance = direction ? 1 : -1;

            if(leaf.Balance == -balance)
            {
                root.Balance = leaf.Balance = 0;
                root = Rotate(root, direction);
            }
            else if(leaf.Balance == balance)
            {
                DoubleRotateRebalance(root, !direction, -balance);
                root = DoubleRotate(root, direction);
            }
            else
            {
                root.Balance = -balance;
                leaf.Balance = balance;
                root = Rotate(root, direction);
                done = true;
            }
            return root;
        }

        protected virtual void DoubleRotateRebalance(IAvlLeaf<TKey, TValue> root, bool direction, int balance)
        {
            var leaf1 = root[direction];
            var leaf2 = leaf1[!direction];

            if (leaf2.Balance == 0)
            {
                root.Balance = leaf1.Balance = 0;
            }
            else if (leaf2.Balance == balance)
            {
                root.Balance = -balance;
                leaf1.Balance = 0;
            }
            else
            {
                root.Balance = 0;
                leaf1.Balance = balance;
            }
            leaf2.Balance = 0;
        }

        protected virtual IAvlLeaf<TKey, TValue> DoubleRotate(IAvlLeaf<TKey, TValue> leaf, bool direction)
        {
            leaf[!direction] = Rotate(leaf[!direction], !direction);
            return Rotate(leaf, direction);
        }

        protected virtual IAvlLeaf<TKey, TValue> Rotate(IAvlLeaf<TKey, TValue> leaf, bool direction)
        {
            var working = leaf[!direction];
            leaf[!direction] = working[direction];
            working[direction] = leaf;
            return working;
        }
    }
}
