using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using BucketProperties = Symbiote.Riak.Impl.Data.BucketProperties;

namespace Symbiote.Riak.Impl
{
    public interface ICommandFactory
    {
        Delete CreateDelete( string bucket, string key, uint minimum );
        Get CreateGet( string bucket, string key, uint minimum );
        GetBucketProperties CreateGetBucketProperties( string bucket );
        GetClientId CreateGetClientId();
        GetServerInfo CreateGetServerInfo();
        ListBuckets CreateListBuckets();
        ListKeys CreateListKeys( string bucket );
        Persist CreatePersistExisting( string bucket, string key, string vectorClock, RiakContent content, uint write, uint dw, bool returnBody );
        Persist CreatePersistNew( string bucket, string key, RiakContent content, uint write, uint dw, bool returnBody );
        Ping CreatePing();
        RunMapReduce CreateMapReduce( string mapReduce, string contentType );
        SetBucketProperties CreateSetBucketProperties( string bucket, BucketProperties properties );
        SetClientId CreateSetClientId( string clientId );
    }
}