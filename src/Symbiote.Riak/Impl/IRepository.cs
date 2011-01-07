using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core.Impl.Futures;
using Symbiote.Riak.Config;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using BucketProperties = Symbiote.Riak.Impl.Data.BucketProperties;

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
            return list.Buckets.Select( x => x.FromBytes() );
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

        public RiakServer(IConnectionProvider connectionProvider)
        {
            ConnectionProvider = connectionProvider;
        }
    }

    public interface IDocumentRepository
    {
        Document<T> GetDocument<T>(string bucket, string key, uint minimum);
        void PersistDocument<T>(string bucket, string key, string vectorClock, Document<T> content, uint write, uint dw, bool returnBody);
    }

    public class DocumentRepository
        : IDocumentRepository
    {
        public IConnectionProvider ConnectionProvider { get; set; }
        public IBasicCommandFactory CommandFactory { get; set; }
        public IRiakConnection Connection { get { return ConnectionProvider.GetConnection(); } }

        public Document<T> GetDocument<T>( string bucket, string key, uint minimum )
        {
            
        }

        public void PersistDocument<T>( string bucket, string key, string vectorClock, Document<T> content, uint write, uint dw, bool returnBody )
        {
            
        }

        public DocumentRepository( IConnectionProvider connectionProvider, IBasicCommandFactory commandFactory )
        {
            ConnectionProvider = connectionProvider;
            CommandFactory = commandFactory;
        }
    }

    public interface IRepository
    {
        void Delete(string bucket, string key, uint minimum);
        T Get<T>(string bucket, string key, uint minimum);
        void Persist<T>(string bucket, string key, string vectorClock, T content, uint write, uint dw, bool returnBody);
    }
}
