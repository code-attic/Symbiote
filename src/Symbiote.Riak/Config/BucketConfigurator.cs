using System;
using System.Collections.Generic;

namespace Symbiote.Riak.Config
{
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

        public void LoadDictionary(IDictionary<Type, BucketConfiguration> lookup)
        {
            Types.ForEach( x => lookup[x] = BucketConfiguration );
        }

        public IConfigureBucket NodesForQuorumRead( uint nodes )
        {
            BucketConfiguration.QuorumReadNodes = nodes;
            return this;
        }

        public IConfigureBucket NodesForQuorumWrite( uint nodes )
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