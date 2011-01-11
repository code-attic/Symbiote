using System.Collections.Generic;
using System.Linq;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using BucketProperties = Symbiote.Riak.Impl.Data.BucketProperties;

namespace Symbiote.Riak.Impl
{
    public class RiakServer
        : IRiakServer
    {
        public IConnectionProvider ConnectionProvider { get; set; }
        public IBasicCommandFactory CommandFactory { get; set; }
        public IRiakConnection Connection { get { return ConnectionProvider.GetConnection(); } }

        public BucketProperties GetBucketProperties( string bucket )
        {
            var command = CommandFactory.CreateGetBucketProperties( bucket );
            var result = Connection.Send( command );
            var properties = result as ProtoBuf.Response.BucketProperties;
            return new BucketProperties( properties.NValue, properties.AllowMultiple );
        }

        public string GetClientId()
        {
            var command = CommandFactory.CreateGetClientId();
            var result = Connection.Send( command );
            var clientId = result as Client;
            return clientId.ClientId.FromBytes();
        }

        public ServerInfo GetServerInfo()
        {
            var command = CommandFactory.CreateGetServerInfo();
            var result = Connection.Send( command );
            var info = result as ServerInformation;
            return new ServerInfo( info.Node.FromBytes(), info.ServerVersion.FromBytes() );
        }

        public IEnumerable<string> GetBucketsList()
        {
            var command = CommandFactory.CreateListBuckets();
            var result = Connection.Send( command );
            var list = result as BucketList;
            return list.Buckets.Select( x => ByteExtensions.FromBytes( x ) );
        }

        public IEnumerable<string> GetKeyList( string bucket )
        {
            var command = CommandFactory.CreateListKeys( bucket );
            var result = Connection.Send( command );
            var list = result as KeyList;
            return list.Keys.Select( x => x.FromBytes() );
        }

        public bool Ping()
        {
            var command = CommandFactory.CreatePing();
            var result = Connection.Send( command );
            return (result as Ping) != null;
        }

        public void SetBucketProperties( string bucket, BucketProperties properties )
        {
            var command = CommandFactory.CreateSetBucketProperties( bucket, properties );
            var result = Connection.Send( command );
            var set = result as BucketPropertiesSet;
        }

        public void SetClientId( string clientId )
        {
            var command = CommandFactory.CreateSetClientId( clientId );
            var result = Connection.Send( command );
        }

        public RiakServer( IConnectionProvider connectionProvider, IBasicCommandFactory commandFactory )
        {
            ConnectionProvider = connectionProvider;
            CommandFactory = commandFactory;
        }
    }
}