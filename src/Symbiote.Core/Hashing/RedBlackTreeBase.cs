namespace Symbiote.Core.Hashing
{
    public abstract class RedBlackTreeBase<TKey, TValue>
    {
        public IRedBlackLeaf<TKey, TValue> Root { get; set; }

        public virtual int Count
        {
            get { return Root.Count; }
        }

        public virtual void Delete(TKey key)
        {
            bool done = false;
            Root = Remove(Root, key, ref done);
            if (!Root.IsEmpty())
                Root.Color = LeafColor.BLACK;
        }

        public virtual TValue Get(TKey key)
        {
            return Root.Get(key);
        }

        public virtual void Add(TKey key, TValue value)
        {
            var leaf = CreateLeaf(key, value);
            Root = Insert(Root, leaf);
            Root.Color = LeafColor.BLACK;
        }

        protected abstract IRedBlackLeaf<TKey, TValue> CreateLeaf(TKey key, TValue value);

        public virtual TValue GetNearest<T>(T key)
        {
            var nearest = Root.Nearest(key);
            if(object.Equals(nearest, null))
            {
                nearest = Root.Value;
            }
            return nearest;
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
                var direction = leaf.GreaterThan(root.Key);
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

                direction = root.LessThan(key);
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
    }
}