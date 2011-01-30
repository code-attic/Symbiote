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
using System;
using System.Collections.Generic;
using System.Linq;
using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Command;
using Symbiote.Redis.Impl.Command.Hash;
using Symbiote.Redis.Impl.Command.List;
using Symbiote.Redis.Impl.Command.Set;
using Symbiote.Redis.Impl.Config;
using Symbiote.Redis.Impl.Serialization;

namespace Symbiote.Redis.Impl
{
    public class RedisClient
        : IDisposable, IRedisClient
    {
        protected int _dbInstance;
        protected RedisConfiguration Configuration { get; set; }
        protected ICacheSerializer Serializer { get; set; }

        public int DbInstance
        {
            get { return _dbInstance; }
            set
            {
                _dbInstance = value;
                var command = new SetDatabaseCommand( value );
                command.Execute();
            }
        }

        public int DbSize
        {
            get
            {
                var command = new DatabaseSizeCommand();
                return command.Execute();
            }
        }

        public string[] Keys
        {
            get
            {
                var command = new KeyListCommand();
                return command.Execute().ToArray();
            }
        }

        public DateTime LastSave
        {
            get
            {
                var command = new LastSaveCommand();
                return command.Execute();
            }
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        public bool Set<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );
            var command = new SetValueCommand<T>( key, value );
            return command.Execute();
        }

        public bool Set<T>( IDictionary<string, T> pairs )
        {
            var set = new SetManyCommand<T>( pairs.Select( p => Tuple.Create( p.Key, p.Value ) ) );
            return set.Execute();
        }

        public bool Set<T>( IEnumerable<T> values, Func<T, string> getKey )
        {
            var set = new SetManyCommand<T>( values.Select( v => Tuple.Create( getKey( v ), v ) ) );
            return set.Execute();
        }

        public bool CheckAndSet<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );

            var command = new CheckAndSetCommand<T>( key, value );
            return command.Execute();
        }

        public T Get<T>( string key )
        {
            ValidateKeyVal( key );

            var command = new GetCommand<T>( key );
            return command.Execute();
        }

        public T GetSet<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );

            var command = new GetAndSetCommand<T>( key, value );
            return command.Execute();
        }

        public bool Remove( params string[] args )
        {
            if ( args == null )
                throw new ArgumentNullException( "args" );
            var command = new RemoveCommand( args );
            return command.Execute();
        }

        public int Increment( string key )
        {
            ValidateKeyVal( key );
            var command = new IncrementCommand( 1, key );
            return command.Execute();
        }

        public int IncrementBy( string key, int count )
        {
            ValidateKeyVal( key );
            var command = new IncrementCommand( count, key );
            return command.Execute();
        }

        public int Decrement( string key )
        {
            ValidateKeyVal( key );
            var command = new DecrementCommand( 1, key );
            return command.Execute();
        }

        public int Decrement( string key, int count )
        {
            ValidateKeyVal( key );
            var command = new DecrementCommand( count, key );
            return command.Execute();
        }

        public bool Rename( string oldKeyname, string newKeyname )
        {
            ValidateInputStringNotNull( oldKeyname, "oldKeyName" );
            ValidateInputStringNotNull( newKeyname, "newKeyName" );
            var command = new RenameCommand( oldKeyname, newKeyname );
            return command.Execute();
        }

        public bool Expire( string key, int seconds )
        {
            ValidateKeyVal( key );
            var command = new ExpireCommand( key, seconds );
            return command.Execute();
        }

        public bool ExpireAt( string key, DateTime time )
        {
            ValidateKeyVal( key );
            var command = new ExpireCommand( key, time );
            return command.Execute();
        }

        public int TimeToLive( string key )
        {
            ValidateKeyVal( key );
            var command = new TimeToLiveCommand( key );
            return command.Execute();
        }

        public void Save()
        {
            var command = new SaveDatabaseCommand( true );
            command.Execute();
        }

        public void BackgroundSave()
        {
            var command = new SaveDatabaseCommand( false );
            command.Execute();
        }

        public void Shutdown()
        {
            var command = new ShutdownCommand();
            command.Execute();
        }

        public void FlushAll()
        {
            var command = new FlushCommand( true );
            command.Execute();
        }

        public void FlushDb()
        {
            var command = new FlushCommand( false );
            command.Execute();
        }

        public Dictionary<string, string> GetInfo()
        {
            var command = new InfoCommand();
            return command.Execute();
        }

        public string[] GetKeys( string pattern )
        {
            var command = new KeyListCommand( pattern );
            return command.Execute().ToArray();
        }


        public int LLen( string key )
        {
            ValidateKeyVal( key );
            var command = new LLenCommand( key );
            return command.Execute();
        }

        public bool LPush<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );
            var command = new LPushCommand<T>( key, value );
            return command.Execute();
        }

        public bool RPush<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );
            var command = new RPushCommand<T>( key, value );
            return command.Execute();
        }

        public T LPop<T>( string key )
        {
            ValidateKeyVal( key );

            var command = new LPopCommand<T>( key );
            return command.Execute();
        }

        public IEnumerable<T> LRange<T>( string key, int startIndex, int endIndex )
        {
            ValidateKeyVal( key );
            var command = new LRangeCommand<T>( key, startIndex, endIndex );
            return command.Execute();
        }


        public bool LTrim( string key, int startIndex, int endIndex )
        {
            ValidateKeyVal( key );
            var command = new LTrimCommand( key, startIndex, endIndex );
            return command.Execute();
        }

        public T LIndex<T>( string key, int index )
        {
            ValidateKeyVal( key );

            var command = new LIndexCommand<T>( key, index );
            return command.Execute();
        }

        public bool LSet<T>( string key, T value, int index )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );
            var command = new LSetCommand<T>( key, value, index );
            return command.Execute();
        }

        public bool LRem<T>( string key, T value, int count )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );
            var command = new LRemCommand<T>( key, value, count );
            return command.Execute();
        }

        public bool RPopLPush( string srcKey, string destKey )
        {
            ValidateInputStringNotNull( srcKey, "srcKey" );
            ValidateInputStringNotNull( destKey, "destKey" );
            var command = new RPopLPushCommand( srcKey, destKey );
            return command.Execute();
        }

        public T RPop<T>( string key )
        {
            ValidateKeyVal( key );

            var command = new RPopCommand<T>( key );
            return command.Execute();
        }

        public bool HSet<T>( string key, string field, T value )
        {
            ValidateKeyVal( key );
            ValidateFieldVal( field );
            ValidateInputValueNotDefault( value );
            var command = new HSetCommand<T>( key, field, value );
            return command.Execute();
        }

        public bool HSet<T>( string key, IDictionary<string, T> values )
        {
            var set = new HSetManyCommand<T>( key, values.Select( p => Tuple.Create( p.Key, p.Value ) ) );
            return set.Execute();
        }

        public T HGet<T>( string key, string field )
        {
            ValidateKeyVal( key );
            ValidateFieldVal( field );

            var command = new HGetCommand<T>( key, field );
            return command.Execute();
        }

        public IEnumerable<T> HGet<T>( string key, IEnumerable<string> values )
        {
            var command = new HGetManyCommand<T>( key, values );
            return command.Execute();
        }

        public int HIncrementBy( string key, string field, int count )
        {
            ValidateKeyVal( key );
            ValidateFieldVal( field );
            var command = new HIncrementCommand( count, key, field );
            return command.Execute();
        }

        public bool HExists( string key, string field )
        {
            ValidateKeyVal( key );
            ValidateFieldVal( field );

            var command = new HExistsCommand( key, field );
            return command.Execute();
        }

        public bool HDel( string key, string field )
        {
            ValidateKeyVal( key );
            ValidateFieldVal( field );

            var command = new HDelCommand( key, field );
            return command.Execute();
        }

        public int HLen( string key )
        {
            ValidateKeyVal( key );
            var command = new HLenCommand( key );
            return command.Execute();
        }

        public IEnumerable<string> HKeys( string key )
        {
            ValidateKeyVal( key );
            var command = new HKeysCommand( key );
            return command.Execute();
        }

        public IEnumerable<T> HVals<T>( string key )
        {
            ValidateKeyVal( key );
            var command = new HValsCommand<T>( key );
            return command.Execute();
        }

        public IDictionary<string, T> HGetAll<T>( string key )
        {
            ValidateKeyVal( key );
            var command = new HGetAllCommand<T>( key );
            return command.Execute();
        }


        public bool SAdd<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );

            var command = new SAddCommand<T>( key, value );
            return command.Execute();
        }

        public IEnumerable<bool> SAdd<T>( IEnumerable<Tuple<string, T>> pairs )
        {
            pairs.ForEach( p =>
                               {
                                   ValidateKeyVal( p.Item1 );
                                   ValidateInputValueNotDefault( p.Item2 );
                               }
                );

            var command = new SAddMultiCommand<T>( pairs );
            return command.Execute();
        }


        public bool SRem<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );

            var command = new SRemCommand<T>( key, value );
            return command.Execute();
        }

        public TValue SPop<TValue>( string key )
        {
            ValidateKeyVal( key );

            var command = new SPopCommand<TValue>( key );
            return command.Execute();
        }

        public bool SMove<T>( string srcKey, string destKey, T value )
        {
            ValidateKeyVal( srcKey );
            ValidateKeyVal( destKey );
            ValidateInputValueNotDefault( value );

            var command = new SMoveCommand<T>( srcKey, destKey, value );
            return command.Execute();
        }

        public int SCard( string key )
        {
            ValidateKeyVal( key );

            var command = new SCardCommand( key );
            return command.Execute();
        }

        public bool SIsMember<T>( string key, T value )
        {
            ValidateKeyVal( key );
            ValidateInputValueNotDefault( value );

            var command = new SIsMemberCommand<T>( key, value );
            return command.Execute();
        }

        public IEnumerable<T> SInter<T>( IEnumerable<string> key )
        {
            var command = new SInterCommand<T>( key );
            return command.Execute();
        }

        public bool SInterStore<T>( IEnumerable<string> key, string destKey )
        {
            var command = new SInterStoreCommand<T>( key, destKey );
            return command.Execute();
        }

        public IEnumerable<T> SUnion<T>( IEnumerable<string> key )
        {
            var command = new SUnionCommand<T>( key );
            return command.Execute();
        }

        public bool SUnionStore<T>( IEnumerable<string> key, string destKey )
        {
            var command = new SUnionStoreCommand<T>( key, destKey );
            return command.Execute();
        }

        public IEnumerable<T> SDiff<T>( IEnumerable<string> key )
        {
            var command = new SDiffCommand<T>( key );
            return command.Execute();
        }

        public bool SDiffStore<T>( IEnumerable<string> key, string destKey )
        {
            var command = new SDiffStoreCommand<T>( key, destKey );
            return command.Execute();
        }

        public IEnumerable<T> SMembers<T>( string key )
        {
            ValidateKeyVal( key );
            var command = new SMembersCommand<T>( key );
            return command.Execute();
        }

        public TValue SRandMember<TValue>( string key )
        {
            ValidateKeyVal( key );

            var command = new SRandMemberCommand<TValue>( key );
            return command.Execute();
        }

        private static void ValidateKeyVal( string key )
        {
            ValidateInputStringNotNull( key, "key" );
        }

        private static void ValidateFieldVal( string field )
        {
            ValidateInputStringNotNull( field, "field" );
        }

        private static void ValidateInputStringNotNull( string val, string exceptionString )
        {
            if ( val == null )
                throw new ArgumentNullException( exceptionString );
        }

        private static void ValidateInputValueNotDefault<T>( T value )
        {
            if ( ReferenceEquals( value, default(T) ) )
                throw new ArgumentNullException( "value" );
        }


        ~RedisClient()
        {
            Dispose( false );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( disposing )
            {
            }
        }

        public RedisClient( RedisConfiguration configuration, ICacheSerializer serializer )
        {
            Configuration = configuration;
            Serializer = serializer;
        }
    }
}