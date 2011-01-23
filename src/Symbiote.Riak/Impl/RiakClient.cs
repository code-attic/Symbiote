/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Riak.Config;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using BucketProperties = Symbiote.Riak.Impl.Data.BucketProperties;
using Symbiote.Core.Impl.Serialization;

namespace Symbiote.Riak.Impl
{
    public class RiakClient :
        IRiakClient,
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

        #region Implementation of IRiakClient

        public bool Delete<T>( string bucket, string key, uint minimumDeletes )
        {
            var command = CommandFactory.CreateDelete( bucket, key, minimumDeletes );
            var response = command.Execute();
            return response != null;
        }

        public Document<T> Get<T>( string bucket, string key, uint reads )
        {
            var command = CommandFactory.CreateGet( bucket, key, reads );
            var getResult = command.Execute();
            if(getResult == null || getResult.Content.Count == 0)
            {
                throw new RiakException( "There was no value available in bucket {0} for the key {1}".AsFormat( bucket, key ) );
            }
            return getResult.Content.FirstOrDefault().ToDocument<T>();
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

        public bool Persist<T>( string bucket, string key, string vectorClock, Document<T> document, uint writeQuorum, uint minimumWrites )
        {
            var riakContent = new RiakContent( document.Value.ToProtocolBuffer(), document.ContentType, document.Charset, document.ContentEncoding, document.VectorClock, document.LastModified, document.LastModifiedInSeconds );
            var command = vectorClock == null
                              ? CommandFactory.CreatePersistNew( bucket, key, riakContent, writeQuorum, minimumWrites, true )
                              : CommandFactory.CreatePersistExisting( bucket, key, vectorClock, riakContent, writeQuorum, minimumWrites, true );

            Persisted result;
            var success = ( result = command.Execute()) != null;
            if(success)
            {
                VectorClockLookup.SetVectorFor(key, result.VectorClock.FromBytes() );
            }
            return success;
        }

        public bool Ping()
        {
            var command = CommandFactory.CreatePing();
            var ping = command.Execute();
            return ping != null;
        }

        public bool SetBucketProperties(string bucket, BucketProperties properties)
        {
            var command = CommandFactory.CreateSetBucketProperties(bucket, properties);
            return ( command.Execute() ) != null;
        }

        public bool SetClientId(string clientId)
        {
            var command = CommandFactory.CreateSetClientId(clientId);
            return (command.Execute()) != null;
        }

        #endregion

        #region Implementation of IKeyValueStore

        public bool Delete<T>( string key )
        {
            var bucket = Configuration.GetBucketForType<T>();
            return Delete<T>( bucket.BucketName, key, bucket.QuorumWriteNodes );
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

        public bool Persist<T>( string key, T instance )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var vectorClock = VectorClockLookup.GetVectorFor( key );
            var document = new Document<T>(instance, vectorClock);
            return Persist( 
                bucket.BucketName, 
                key, 
                vectorClock, 
                document, 
                bucket.QuorumWriteNodes, bucket.QuorumWriteNodes );
        }

        #endregion

        #region Implementation of IRepository

        public bool Delete<T>( T instance )
        {
            //do nuffin
            return false;
        }

        public bool Persist<T>( T instance )
        {
            // do nuffin
            return false;
        }

        #endregion

        #region Implementation of IDocumentRepository

        public bool DeleteDocument<T>( string key )
        {
            return Delete<T>( key );
        }

        public Document<T> GetDocument<T>( string key )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var command = CommandFactory.CreateGet( bucket.BucketName, key, bucket.QuorumReadNodes );
            var getResult = command.Execute();
            var document = getResult.Content.FirstOrDefault().ToDocument<T>();
            return document;
        }

        public IEnumerable<Document<T>> GetAllDocuments<T>()
        {
            var bucket = Configuration.GetBucketForType<T>();
            var command = CommandFactory.CreateListKeys(bucket.BucketName);
            var keys = command.Execute();
            return keys.Keys.Select(x => GetDocument<T>(x.FromBytes()));
        }

        public bool PersistDocument<T>( string key, Document<T> document )
        {
            var bucket = Configuration.GetBucketForType<T>();
            return Persist( bucket.BucketName, key, document.VectorClock, document, bucket.QuorumWriteNodes, bucket.QuorumReadNodes );
        }

        #endregion
    }
}