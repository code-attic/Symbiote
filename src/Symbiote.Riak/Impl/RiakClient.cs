using System.Collections.Generic;
using System.Linq;
using Symbiote.Riak.Config;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using BucketProperties = Symbiote.Riak.Impl.Data.BucketProperties;

namespace Symbiote.Riak.Impl
{
    public class RiakClient :
        IRiakServer,
        IKeyValueStore,
        IRepository,
        IDocumentRepository
        
    {
        public ICommandFactory CommandFactory { get; set; }
        public ITrackVectors VectorClockLookup { get; set; }
        public IRiakConfiguration Configuration { get; set; }

        public RiakClient( ICommandFactory commandFactory, ITrackVectors vectorClockLookup, IRiakConfiguration configuration )
        {
            CommandFactory = commandFactory;
            VectorClockLookup = vectorClockLookup;
            Configuration = configuration;
        }

        #region Implementation of IRiakServer

        public void Delete<T>( string bucket, string key, uint minimumDeletes )
        {
            var command = CommandFactory.CreateDelete( bucket, key, minimumDeletes );
            var response = command.Execute();
        }

        public Document<T> Get<T>( string bucket, string key, uint reads )
        {
            var command = CommandFactory.CreateGet( bucket, key, reads );
            return command.Execute().ToDocument<T>();
        }

        public BucketProperties GetBucketProperties(string bucket)
        {
            var command = CommandFactory.CreateGetBucketProperties(bucket);
            var properties = command.Execute();
            return new BucketProperties(properties.NValue, properties.AllowMultiple);
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
            return new ServerInfo(info.Node.FromBytes(), info.ServerVersion.FromBytes());
        }

        public IEnumerable<string> GetBucketsList()
        {
            var command = CommandFactory.CreateListBuckets();
            var list = command.Execute();
            return list.Buckets.Select(x => ByteExtensions.FromBytes(x));
        }

        public IEnumerable<string> GetKeyList(string bucket)
        {
            var command = CommandFactory.CreateListKeys(bucket);
            var list = command.Execute();
            return list.Keys.Select(x => x.FromBytes());
        }

        public void Persist<T>( string bucket, string key, string vectorClock, Document<T> document, uint writeQuorum, uint minimumWrites )
        {
            var riakContent = new RiakContent( document.Value, document.ContentType, document.Charset, document.ContentEncoding, document.VectorClock, document.LastModified, document.LastModifiedInSeconds );
            var command = CommandFactory.CreatePersist( bucket, key, vectorClock, riakContent, writeQuorum, minimumWrites, false );
        }

        public bool Ping()
        {
            var command = CommandFactory.CreatePing();
            var ping = command.Execute();
            return ping != null;
        }

        public void SetBucketProperties(string bucket, BucketProperties properties)
        {
            var command = CommandFactory.CreateSetBucketProperties(bucket, properties);
            command.Execute();
        }

        public void SetClientId(string clientId)
        {
            var command = CommandFactory.CreateSetClientId(clientId);
            command.Execute();
        }

        #endregion

        #region Implementation of IKeyValueStore

        public void Delete<T>( string key )
        {
            var bucket = Configuration.GetBucketForType<T>();
            Delete<T>( bucket.BucketName, key, bucket.QuorumWriteNodes );
        }

        public T Get<T>( string key )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var document = Get<T>( bucket.BucketName, key, bucket.QuorumReadNodes );
            VectorClockLookup.SetVectorFor( key, document.VectorClock );
            return document.Value;
        }

        public IEnumerable<T> GetAll<T>()
        {
            var bucket = Configuration.GetBucketForType<T>();
            var command = CommandFactory.CreateListKeys( bucket.BucketName );
            var keys = command.Execute();
            return keys.Keys.Select( x => Get<T>( x.FromBytes() ) );
        }

        public void Persist<T>( string key, T instance )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var vectorClock = VectorClockLookup.GetVectorFor( key );
            var document = new Document<T>(instance, vectorClock);
            Persist( 
                bucket.BucketName, 
                key, 
                vectorClock, 
                document, 
                bucket.QuorumWriteNodes, bucket.QuorumWriteNodes );
        }

        #endregion

        #region Implementation of IRepository

        public void Delete<T>( T instance )
        {
            //do nuffin    
        }

        public void Persist<T>( T instance )
        {
            // do nuffin
        }

        #endregion

        #region Implementation of IDocumentRepository

        public void DeleteDocument<T>( string key )
        {
            Delete<T>( key );
        }

        public Document<T> GetDocument<T>( string key )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var command = CommandFactory.CreateGet( bucket.BucketName, key, bucket.QuorumReadNodes );
            var document = command.Execute().ToDocument<T>();
            return document;
        }

        public IEnumerable<Document<T>> GetAllDocuments<T>()
        {
            var bucket = Configuration.GetBucketForType<T>();
            var command = CommandFactory.CreateListKeys(bucket.BucketName);
            var keys = command.Execute();
            return keys.Keys.Select(x => GetDocument<T>(x.FromBytes()));
        }

        public void PersistDocument<T>( string key, Document<T> document )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var content = new RiakContent(
                document.Value,
                document.ContentType,
                document.Charset,
                document.ContentEncoding,
                document.VectorClock,
                document.LastModified,
                document.LastModifiedInSeconds
                );
            var command = CommandFactory.CreatePersist( bucket.BucketName, key, document.VectorClock, content, bucket.QuorumWriteNodes, bucket.QuorumWriteNodes, true );
            var result = command.Execute();
            VectorClockLookup.SetVectorFor(key, result.VectorClock.FromBytes());
        }

        #endregion
    }
}