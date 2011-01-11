using System.Collections.Generic;
using Symbiote.Riak.Impl.Data;

namespace Symbiote.Riak.Impl
{
    public interface IRiakServer
    {
        BucketProperties GetBucketProperties(string bucket);
        string GetClientId();
        ServerInfo GetServerInfo();
        IEnumerable<string> GetBucketsList();
        IEnumerable<string> GetKeyList(string bucket);
        bool Ping();
        void SetBucketProperties(string bucket, BucketProperties properties);
        void SetClientId(string clientId);
    }
}