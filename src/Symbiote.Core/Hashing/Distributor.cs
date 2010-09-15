using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Symbiote.Core.Extensions;

namespace Symbiote.Core.Hashing
{
    public class Distributor<TNode>
    {
        protected IHashingProvider HashProvider { get; set; }
        protected ReaderWriterLockSlim MapLock { get; set; }
        protected HashedRedBlackTree<string, string> Map { get; set; }
        protected ConcurrentDictionary<string, List<string>> AliasLookup { get; set; }
        protected ConcurrentDictionary<string, TNode> Nodes { get; set; }

        protected int AliasCount { get; set; }

        public void AddNode(string name, TNode node)
        {
            Nodes[name] = node;
            BuildAliases(name);
        }

        protected void BuildAliases(string name)
        {
            var aliasList = new List<string>(AliasCount);
            for (int i = 0; i < AliasCount; i++)
            {
                try
                {
                    var alias = "{0}_{1}".AsFormat(name, i);
                    var hash = HashProvider.Hash(alias);
                    aliasList.Add(alias);
                    MapLock.EnterWriteLock();
                    Map.Add(alias, name);
                }
                finally
                {
                    MapLock.ExitWriteLock();
                }
            }
            AliasLookup[name] = aliasList;
        }

        protected void RemoveAliases(string name)
        {
            var aliasList = new List<string>();
            if(AliasLookup.TryRemove(name, out aliasList))
            {
                foreach (var alias in aliasList)
                {
                    try
                    {
                        MapLock.EnterWriteLock();
                        Map.Delete(alias);
                    }
                    finally
                    {
                        MapLock.ExitWriteLock();
                    }
                }
            }
        }

        public void RemoveNode(string name)
        {
            TNode node = default(TNode);
            Nodes.TryRemove(name, out node);
            RemoveAliases(name);
        }

        public TNode GetNode<TKey>(TKey key)
        {
            var hashKey = HashProvider.Hash(key);
            string nodeKey = "";
            try
            {
                MapLock.EnterReadLock();
                nodeKey = Map.GetNearest(hashKey);
            }
            finally
            {
                MapLock.ExitReadLock();
            }
            return Nodes[nodeKey];
        }

        public Distributor(int aliasCount)
        {
            AliasCount = aliasCount;
            Map = new HashedRedBlackTree<string, string>();
            AliasLookup = new ConcurrentDictionary<string, List<string>>(3, 10);
            Nodes = new ConcurrentDictionary<string, TNode>(3, 10);
            MapLock = new ReaderWriterLockSlim();
            HashProvider = new MD5HashProvider();
        }
    }
}
