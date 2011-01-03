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
using System.Threading;

namespace Symbiote.Core.Hashing.Impl
{
    public abstract class RedBlackTreeBase<TKey, TValue>
    {
        protected ReaderWriterLockSlim Lock { get; set; }
        public IRedBlackLeaf<TKey, TValue> Root { get; set; }

        public virtual int Count
        {
            get { return Root.Count; }
        }

        public virtual void Delete(TKey key)
        {
            Lock.EnterWriteLock();

            bool done = false;
            Root = Remove(Root, key, ref done);
            if (!Root.IsEmpty())
                Root.Color = LeafColor.BLACK;

            Lock.ExitWriteLock();
        }

        public virtual TValue Get(TKey key)
        {
            Lock.EnterReadLock();
            IRedBlackLeaf<TKey, TValue> leaf = Root;
            var compare = CreateLeaf(key, default(TValue));
            while (leaf != null)
            {
                if (leaf.GreaterThan(compare))
                {
                    leaf = leaf.Left;
                }
                else if (leaf.LessThan(compare))
                {
                    leaf = leaf.Right;
                }
                else
                {
                    break;
                }
            }
            Lock.ExitReadLock();
            return leaf.IsEmpty() ? default(TValue) : leaf.Value;
        }

        public virtual void Add(TKey key, TValue value)
        {
            Lock.EnterWriteLock();
            var leaf = CreateLeaf(key, value);
            Root = Insert(Root, leaf);
            Root.Color = LeafColor.BLACK;
            Lock.ExitWriteLock();
        }

        protected abstract IRedBlackLeaf<TKey, TValue> CreateLeaf(TKey key, TValue value);

        public virtual TValue GetNearest<T>(T key)
        {
            IRedBlackLeaf<TKey, TValue> leaf = Root;
            IRedBlackLeaf<TKey, TValue> last = null;
            var compareKey = (TKey) Convert.ChangeType(key, typeof (TKey));
            var compare = CreateLeaf(compareKey, default(TValue));

            while (leaf != null)
            {
                if (leaf.GreaterThan(compare))
                {
                    leaf = leaf.Left;
                }
                else if (leaf.LessThan(compare))
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

        protected IRedBlackLeaf<TKey, TValue> GetMaxLeaf()
        {
            IRedBlackLeaf<TKey, TValue> leaf;
            leaf = this.Root;

            if (leaf == null)
                return null;

            while (leaf.Right != null)
            {
                leaf = leaf.Right;
            }
            return leaf;
        }

        protected virtual IRedBlackLeaf<TKey, TValue> Insert(IRedBlackLeaf<TKey, TValue> root, IRedBlackLeaf<TKey, TValue> leaf)
        {
            if (root.IsEmpty())
            {
                root = leaf;
            }
            else
            {
                leaf.Parent = root;
                var direction = leaf.GreaterThan(root);
                root[direction] = Insert(root[direction], leaf);
                root = BalancePostInsert(root, direction);
            }
            return root;
        }

        protected virtual IRedBlackLeaf<TKey, TValue> Remove(IRedBlackLeaf<TKey, TValue> root, TKey key, ref bool done)
        {
            if(root.IsEmpty())
            {
                done = true;
            }
            else
            {
                bool direction;
                if(root.Key.Equals(key))
                {
                    if(root.Left.IsEmpty() || root.Right.IsEmpty())
                    {
                        var save = root[root.Left.IsEmpty()];

                        if (root.IsRed())
                        {
                            done = true;
                        }
                        else if(save.IsRed())
                        {
                            save.Color = LeafColor.BLACK;
                            done = true;
                        }

                        return save;
                    }
                    else
                    {
                        var heir = root.Left;
                        while (!heir.Right.IsEmpty())
                            heir = heir.Right;

                        root.Value = heir.Value;
                        root.Key = heir.Key;
                        key = heir.Key;
                    }
                }

                direction = root.LessThan(CreateLeaf(key, default(TValue)));
                root[direction] = Remove(root[direction], key, ref done);

                if (!done)
                    root = BalancePostDelete(root, direction, ref done);
            }
            return root;
        }

        protected virtual IRedBlackLeaf<TKey, TValue> BalancePostInsert(IRedBlackLeaf<TKey, TValue> root, bool direction)
        {
            if (root[direction].IsRed())
            {
                if (root[!direction].IsRed())
                {
                    root.Color = LeafColor.RED;
                    root.Left.Color = LeafColor.BLACK;
                    root.Right.Color = LeafColor.BLACK;
                }
                else
                {
                    if (root[direction][direction].IsRed())
                        root = Rotate(root, !direction);
                    else if (root[direction][!direction].IsRed())
                        root = DoubleRotate(root, !direction);
                }
            }
            return root;
        }

        protected virtual IRedBlackLeaf<TKey, TValue> BalancePostDelete(IRedBlackLeaf<TKey, TValue> root, bool direction, ref bool done)
        {
            IRedBlackLeaf<TKey, TValue> worker = root;
            IRedBlackLeaf<TKey, TValue> leaf = root[!direction];

            if(leaf.IsRed())
            {
                root = Rotate(root, direction);
                leaf = worker[!direction];
            }

            if(!leaf.IsEmpty())
            {
                if(leaf.Left.IsBlack() && leaf.Right.IsBlack())
                {
                    if (worker.IsRed())
                        done = true;
                    worker.Color = LeafColor.BLACK;
                    leaf.Color = LeafColor.RED;
                }
                else
                {
                    var save = worker.IsRed();
                    var newRoot = root.Key.Equals(worker.Key);
                    if (leaf[direction].IsRed())
                        worker = Rotate(worker, direction);
                    else
                        worker = DoubleRotate(worker, direction);

                    worker.Color = save ? LeafColor.RED : LeafColor.BLACK;
                    worker.Left.Color = LeafColor.BLACK;
                    worker.Right.Color = LeafColor.BLACK;

                    if (newRoot)
                        root = worker;
                    else
                        root[direction] = worker;

                    done = true;
                }
            }
            return root;
        }

        protected virtual IRedBlackLeaf<TKey, TValue> DoubleRotate(IRedBlackLeaf<TKey, TValue> leaf, bool direction)
        {
            leaf[!direction] = Rotate(leaf[!direction], !direction);
            return Rotate(leaf, direction);
        }

        protected virtual IRedBlackLeaf<TKey, TValue> Rotate(IRedBlackLeaf<TKey, TValue> leaf, bool direction)
        {
            var working = leaf[!direction];
            
            leaf[!direction] = working[direction];
            working[direction] = leaf;

            leaf.Color = LeafColor.RED;
            working.Color = LeafColor.BLACK;

            return working;
        }

        protected RedBlackTreeBase()
        {
            Lock = new ReaderWriterLockSlim();
        }
    }
}