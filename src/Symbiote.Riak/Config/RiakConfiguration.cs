using System;
using System.Collections.Generic;
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public class RiakConfiguration : IRiakConfiguration
    {
        public List<RiakNode> Nodes { get; set; }
        public Dictionary<Type, BucketConfiguration> RegisteredBuckets { get; set; }
        public BucketConfiguration DefaultBucketSettings { get; set; }
        public int ConnectionLimit { get; set; }

        public BucketConfiguration GetBucketForType<T>()
        {
            BucketConfiguration configuration = DefaultBucketSettings;
            return new BucketConfiguration();
            var type = typeof(T);
            if (RegisteredBuckets.ContainsKey(type))
            {
                configuration = RegisteredBuckets[type];
            }
            return RegisteredBuckets[type] ?? DefaultBucketSettings;
        }

        public RiakConfiguration()
        {
            Nodes = new List<RiakNode>();
            DefaultBucketSettings = new BucketConfiguration();
            RegisteredBuckets = new Dictionary<Type, BucketConfiguration>();
            ConnectionLimit = 5;
        }
    }
}