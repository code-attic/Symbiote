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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Symbiote.Core.Extensions;
using Symbiote.Core.Hashing.Impl;
using System.Linq;

namespace Symbiote.Core.Hashing
{
    /// <summary>
    /// A consistent hashing facility intended to make keyspace mapping problems
    /// simple to solve. Includes support for live rebalancing.
    /// </summary>
    public class Distributor<TNode>
    {
        protected IHashingProvider HashProvider { get; set; }
        protected ReaderWriterLockSlim MapLock { get; set; }
        protected RedBlackTree<long, string> Map { get; set; }
        protected ConcurrentDictionary<string, List<long>> AliasLookup { get; set; }
        protected ConcurrentDictionary<string, TNode> Nodes { get; set; }

        public int AliasCount { get; protected set; }

        /// <summary>
        /// Adds a node to the keyspace by a given name.
        /// </summary>
        public virtual void AddNode(string name, TNode node)
        {
            Nodes[name] = node;
            BuildAliases(name);
        }

        /// <summary>
        /// Rebalances a node's virtual key space by a delta of 'change'
        /// </summary>
        public virtual void RebalanceNodeBy(string name, int change)
        {
            if(change > 0)
                AddAliases( name, change );
            else
                RemoveKeySpaceFrom( name, change * -1 );
        }

        /// <summary>
        /// Rebalances a node's virtual key space to some total number
        /// </summary>
        public virtual void RebalanceNodeTo(string name, int total)
        {
            List<long> aliasList;
            if(!AliasLookup.TryGetValue(name, out aliasList))
            {
                aliasList = new List<long>(total);
                AliasLookup[name] = aliasList;
            }
            
            if(total != aliasList.Count)
            {
                RebalanceNodeBy(name, total - aliasList.Count);
            }
        }
        
        protected void AddAliases(string name, int count)
        {
            List<long> aliasList;
            if (!AliasLookup.TryGetValue(name, out aliasList))
            {
               aliasList = new List<long>(count);
               AliasLookup[name] = aliasList;
            }

            var total = aliasList.Count;
            for (int i = total; i <= total + count; i++)
            {
                try
                {
                    var alias = HashProvider.Hash("{0}_{1}".AsFormat(name, i));
                    aliasList.Add(alias);
                    MapLock.EnterWriteLock();
                    Map.Add(alias, name);
                }
                finally
                {
                    MapLock.ExitWriteLock();
                }
            }
        }

        protected void BuildAliases(string name)
        {
            AddAliases( name, AliasCount );
        }

        protected void RemoveKeySpaceFrom(string name, int count)
        {
            List<long> aliasList;
            if(AliasLookup.TryGetValue( name, out aliasList ))
            {
                var removals = aliasList.Take( count );
                removals
                    .ForEach( x =>
                    {
                        try
                        {
                            aliasList.Remove( x );
                            MapLock.EnterWriteLock();
                            Map.Delete( x );
                        }
                        finally
                        {
                            MapLock.ExitWriteLock();
                        }
                    } );
            }
        }

        protected void RemoveAliases(string name)
        {
            List<long> aliasList;
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

        /// <summary>
        /// Removes the entire node and all keyspace associated with it
        /// </summary>
        public virtual void RemoveNode(string name)
        {
            TNode node = default(TNode);
            Nodes.TryRemove(name, out node);
            RemoveAliases(name);
        }

        /// <summary>
        /// Retrieves the node value through key space collision by hashing
        /// TKey to some value and mapping that to the underlying virtual key space
        /// </summary>
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

        /// <summary>
        /// Initializes a consistent hashing key space where the alias count is the
        /// number of default virtual nodes assigned to a given node
        /// </summary>
        public Distributor(int aliasCount)
        {
            AliasCount = aliasCount;
            Map = new RedBlackTree<long, string>();
            AliasLookup = new ConcurrentDictionary<string, List<long>>(3, 10);

            Nodes = new ConcurrentDictionary<string, TNode>(3, 10);
            MapLock = new ReaderWriterLockSlim();
            HashProvider = new MD5HashProvider();
        }
    }
}
