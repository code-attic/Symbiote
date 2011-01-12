using System;
using System.Collections.Generic;
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public class RiakConfigurator
    {
        public RiakConfiguration Configuration { get; protected set; }
        
        public RiakConfigurator AddLocalNode()
        {
            Configuration.Nodes.Add( new RiakNode() );
            return this;
        }

        public RiakConfigurator AddNode( Action<RiakNode> riakNodeConfiguration )
        {
            var node = new RiakNode();
            riakNodeConfiguration( node );
            Configuration.Nodes.Add( node );
            return this;
        }

        public RiakConfigurator RegisterBucket(string bucketName, Action<IConfigureBucket> assignments)
        {
            var assignment = new BucketConfigurator( bucketName );
            assignments( assignment );
            assignment.LoadDictionary( Configuration.RegisteredBuckets );
            return this;
        }

        public RiakConfigurator()
        {
            Configuration = new RiakConfiguration();
        }
    }

    public interface IConfigureBucket
    {
        IConfigureBucket Assign<T>();
        IConfigureBucket NodesForQuorumRead( int nodes );
        IConfigureBucket NodesForQuorumWrite( int nodes );
        IConfigureBucket WaitOnWritesBeforeAck();

    }

    public class BucketConfiguration
    {
        public int QuorumReadNodes { get; set; }
        public int QuorumWriteNodes { get; set; }
        public bool WaitOnWrites { get; set; }

        public BucketConfiguration()
        {
            QuorumReadNodes = 1;
            QuorumWriteNodes = 1;

        }
    }

    public class BucketConfigurator
        : IConfigureBucket
    {
        public string BucketName { get; set; }
        public List<Type> Types { get; set; }
        public BucketConfiguration BucketConfiguration { get; set; }

        public IConfigureBucket Assign<T>()
        {
            Types.Add(typeof(T));
            return this;
        }

        public void LoadDictionary(IDictionary<Type, string> lookup)
        {
            Types.ForEach( x => lookup[x] = BucketName );
        }

        public IConfigureBucket NodesForQuorumRead( int nodes )
        {
            BucketConfiguration.QuorumReadNodes = nodes;
            return this;
        }

        public IConfigureBucket NodesForQuorumWrite( int nodes )
        {
            BucketConfiguration.QuorumWriteNodes = nodes;
            return this;
        }

        public IConfigureBucket WaitOnWritesBeforeAck()
        {
            BucketConfiguration.WaitOnWrites = true;
            return this;
        }

        public BucketConfigurator( string bucketName )
        {
            BucketName = bucketName;
            Types = new List<Type>();
        }
    }
}