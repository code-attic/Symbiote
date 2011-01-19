using System.Collections.Generic;
using Symbiote.Riak.Impl.Data;

namespace Symbiote.Riak.Impl
{
    public interface IRiakServer
    {
        void Delete<T>( string bucket, string key, uint minimumDeletes );
        BucketProperties GetBucketProperties( string bucket );
        Document<T> Get<T>( string bucket, string key, uint reads );
        string GetClientId();
        ServerInfo GetServerInfo();
        IEnumerable<string> GetBucketsList();
        IEnumerable<string> GetKeyList( string bucket );
        bool Ping();
        void Persist<T>( string bucket, string key, string vectorClock, Document<T> document, uint writeQuorum, uint minimumWrites );
        void SetBucketProperties( string bucket, BucketProperties properties );
        void SetClientId( string clientId );
    }
}