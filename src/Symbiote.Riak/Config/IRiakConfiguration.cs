using System;
using System.Collections.Generic;
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public interface IRiakConfiguration
    {
        List<RiakNode> Nodes { get; set; }
        int ConnectionLimit { get; set; }
        BucketConfiguration GetBucketForType<T>();
    }
}