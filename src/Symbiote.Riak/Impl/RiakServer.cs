using System.Collections.Generic;
using System.Linq;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf;
using BucketProperties = Symbiote.Riak.Impl.Data.BucketProperties;

namespace Symbiote.Riak.Impl
{
    public class RiakServer
        : IRiakServer
    {
        public IBasicCommandFactory CommandFactory { get; set; }

        public BucketProperties GetBucketProperties( string bucket )
        {
            var command = CommandFactory.CreateGetBucketProperties( bucket );
            var properties = command.Execute();
            return new BucketProperties( properties.NValue, properties.AllowMultiple );
        }

        public string GetClientId()
        {
            var command = CommandFactory.CreateGetClientId();
            var clientId = command.Execute();
            return clientId.ClientId.FromBytes();
        }

        public ServerInfo GetServerInfo()
        {
            var command = CommandFactory.CreateGetServerInfo();
            var info = command.Execute();
            return new ServerInfo( info.Node.FromBytes(), info.ServerVersion.FromBytes() );
        }

        public IEnumerable<string> GetBucketsList()
        {
            var command = CommandFactory.CreateListBuckets();
            var list = command.Execute();
            return list.Buckets.Select( x => ByteExtensions.FromBytes( x ) );
        }

        public IEnumerable<string> GetKeyList( string bucket )
        {
            var command = CommandFactory.CreateListKeys( bucket );
            var list = command.Execute();
            return list.Keys.Select( x => x.FromBytes() );
        }

        public bool Ping()
        {
            var command = CommandFactory.CreatePing();
            var ping = command.Execute();
            return ping != null;
        }

        public void SetBucketProperties( string bucket, BucketProperties properties )
        {
            var command = CommandFactory.CreateSetBucketProperties( bucket, properties );
            command.Execute();
        }

        public void SetClientId( string clientId )
        {
            var command = CommandFactory.CreateSetClientId( clientId );
            command.Execute();
        }

        public RiakServer( IBasicCommandFactory commandFactory )
        {
            CommandFactory = commandFactory;
        }
    }
}