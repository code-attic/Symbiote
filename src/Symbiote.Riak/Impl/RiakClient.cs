// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Persistence;
using Symbiote.Core.Serialization;
using Symbiote.Riak.Config;
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf.Response;
using ByteExtensions = Symbiote.Riak.Impl.ProtoBuf.ByteExtensions;

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
        public IKeyAccessor KeyAccessor { get; set; }

        public RiakClient( ICommandFactory commandFactory, ITrackVectors vectorClockLookup,
                           IRiakConfiguration configuration )
        {
            CommandFactory = commandFactory;
            VectorClockLookup = vectorClockLookup;
            Configuration = configuration;
        }

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
            if ( getResult == null || getResult.Content.Count == 0 )
            {
                throw new RiakException( "There was no value available in bucket {0} for the key {1}".AsFormat( bucket,
                                                                                                                key ) );
            }
            var riakContent = getResult.Content.FirstOrDefault();
            return riakContent.ToDocument<T>();
        }

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
            return ByteExtensions.FromBytes( clientId.ClientId );
        }

        public ServerInfo GetServerInfo()
        {
            var command = CommandFactory.CreateGetServerInfo();
            var info = command.Execute();
            return new ServerInfo( ByteExtensions.FromBytes( info.Node ), ByteExtensions.FromBytes( info.ServerVersion ) );
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
            return list.Keys.Select( x => ByteExtensions.FromBytes( x ) ).ToList();
        }

        public bool Persist<T>( string bucket, string key, string vectorClock, Document<T> document, uint writeQuorum,
                                uint minimumWrites )
        {
            var riakContent = new RiakContent( document.Value.ToProtocolBuffer(), document.ContentType, document.Charset,
                                               document.ContentEncoding, document.VectorClock, document.LastModified,
                                               document.LastModifiedInSeconds );
            var command = vectorClock == null
                              ? CommandFactory.CreatePersistNew( bucket, key, riakContent, writeQuorum, minimumWrites,
                                                                 true )
                              : CommandFactory.CreatePersistExisting( bucket, key, vectorClock, riakContent, writeQuorum,
                                                                      minimumWrites, true );

            Persisted result;
            var success = (result = command.Execute()) != null;
            if ( success )
            {
                VectorClockLookup.SetVectorFor( key, ByteExtensions.FromBytes( result.VectorClock ) );
            }
            return success;
        }

        public bool Ping()
        {
            var command = CommandFactory.CreatePing();
            var ping = command.Execute();
            return ping != null;
        }

        public bool SetBucketProperties( string bucket, BucketProperties properties )
        {
            var command = CommandFactory.CreateSetBucketProperties( bucket, properties );
            return (command.Execute()) != null;
        }

        public bool SetClientId( string clientId )
        {
            var command = CommandFactory.CreateSetClientId( clientId );
            return (command.Execute()) != null;
        }

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
            return keys
                .Keys
                .Select( x => Get<T>( ByteExtensions.FromBytes( x ) ) )
                .ToList();
        }

        public bool Persist<T>( string key, T instance )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var vectorClock = VectorClockLookup.GetVectorFor( key );
            var document = new Document<T>( instance, vectorClock );
            return Persist(
                bucket.BucketName,
                key,
                vectorClock,
                document,
                bucket.QuorumWriteNodes, bucket.QuorumWriteNodes );
        }

        public bool Delete<T>( T instance ) where T : class
        {
            return Delete<T>( KeyAccessor.GetId( instance ) );
        }

        public bool Persist<T>( T instance ) where T : class
        {
            return Persist( KeyAccessor.GetId( instance ), instance );
        }

        public bool DeleteDocument<T>( string key )
        {
            return Delete<T>( key );
        }

        public Document<T> GetDocument<T>( string key )
        {
            var bucket = Configuration.GetBucketForType<T>();
            var command = CommandFactory.CreateGet( bucket.BucketName, key, bucket.QuorumReadNodes );
            var getResult = command.Execute();
            var riakContent = getResult.Content.FirstOrDefault();
            var document = riakContent.ToDocument<T>();
            return document;
        }

        public IEnumerable<Document<T>> GetAllDocuments<T>()
        {
            var bucket = Configuration.GetBucketForType<T>();
            var command = CommandFactory.CreateListKeys( bucket.BucketName );
            var keys = command.Execute();
            return keys.Keys.Select( x => GetDocument<T>( ByteExtensions.FromBytes( x ) ) );
        }

        public bool PersistDocument<T>( string key, Document<T> document )
        {
            var bucket = Configuration.GetBucketForType<T>();
            return Persist( bucket.BucketName, key, document.VectorClock, document, bucket.QuorumWriteNodes,
                            bucket.QuorumReadNodes );
        }
    }
}