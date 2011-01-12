using System;
using System.Collections.Generic;
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public class RiakConfiguration : IRiakConfiguration
    {
        public List<RiakNode> Nodes { get; set; }
        public Dictionary<Type, string> RegisteredBuckets { get; set; }

        public string GetBucketForType<T>()
        {
            var type = typeof(T);
            return RegisteredBuckets.ContainsKey( type )
                       ? RegisteredBuckets[type]
                       : type.Name;
        }

        public RiakConfiguration()
        {
            Nodes = new List<RiakNode>();
        }
    }
}