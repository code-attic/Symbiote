using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using BucketProperties = Symbiote.Riak.Impl.Data.BucketProperties;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public class ProtoBufCommandFactory : ICommandFactory
    {
        public Delete CreateDelete( string bucket, string key, uint minimum )
        {
            return new Delete( bucket, key, minimum );
        }

        public Get CreateGet( string bucket, string key, uint minimum )
        {
            return new Get( bucket, key, minimum );
        }

        public GetBucketProperties CreateGetBucketProperties( string bucket )
        {
            return new GetBucketProperties( bucket );
        }

        public GetClientId CreateGetClientId()
        {
            return new GetClientId();
        }

        public GetServerInfo CreateGetServerInfo()
        {
            return new GetServerInfo();
        }

        public ListBuckets CreateListBuckets()
        {
            return new ListBuckets();
        }

        public ListKeys CreateListKeys( string bucket )
        {
            return new ListKeys( bucket );
        }

        public Persist CreatePersist( string bucket, string key, string vectorClock, RiakContent content, uint write, uint dw, bool returnBody )
        {
            return new Persist( bucket, key, vectorClock, content, write, dw, returnBody );
        }

        public Ping CreatePing()
        {
            return new Ping();
        }

        public RunMapReduce CreateMapReduce( string mapReduce, string contentType )
        {
            return new RunMapReduce( mapReduce, contentType );
        }

        public SetBucketProperties CreateSetBucketProperties( string bucket, BucketProperties properties )
        {
            return new SetBucketProperties( bucket, properties );
        }

        public SetClientId CreateSetClientId( string clientId )
        {
            return new SetClientId( clientId );
        }
    }
}