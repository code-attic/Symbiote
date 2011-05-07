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

namespace Symbiote.Redis
{
    public interface IRedisClient
    {
        int DbSize { get; }
        bool Set<T>( string key, T value );
        bool Set<T>( IDictionary<string, T> pairs );
        bool Set<T>( IEnumerable<T> values, Func<T, string> getKey );
        bool CheckAndSet<T>( string key, T value );
        T Get<T>( string key );
        T GetSet<T>( string key, T value );
        bool Remove( params string[] args );
        int Increment( string key );
        int Increment( string key, int count );
        int Decrement( string key );
        int Decrement( string key, int count );
        bool Exists(string key);
        bool Rename( string oldKeyname, string newKeyname );
        bool Expire( string key, int seconds );
        bool ExpireAt( string key, DateTime time );
        int TimeToLive( string key );
        bool Save();
        bool BackgroundSave();
        void Shutdown();
        void FlushAll();
        void FlushDb();
        Dictionary<string, string> GetInfo();
        string[] GetKeys( string pattern );
        string[] Keys { get; }
        DateTime LastSave { get; }

        //Connection commands
        bool SelectDb(int dbIndex);

        //List commands
        int LLen( string key );
        IEnumerable<T> LRange<T>( string key, int startIndex, int endIndex );
        bool LPush<T>( string key, T value );
        bool RPush<T>( string key, T value );
        T LPop<T>( string key );
        T RPop<T>( string key );
        bool LTrim( string key, int startIndex, int endIndex );
        T LIndex<T>( string key, int index );
        bool LSet<T>( string key, T value, int index );
        bool LRem<T>( string key, T value, int count );
        bool RPopLPush( string srcKey, string destKey );

        //Hash commands
        bool HSet<T>( string key, string field, T value );
        bool HSet<T>( string key, IDictionary<string, T> values );
        T HGet<T>( string key, string field );
        IEnumerable<T> HGet<T>( string key, IEnumerable<string> values );
        int HIncrementBy( string key, string field, int count );
        bool HExists( string key, string field );
        bool HDel( string key, string field );
        int HLen( string key );
        IEnumerable<string> HKeys( string key );
        IEnumerable<T> HVals<T>( string key );
        IDictionary<string, T> HGetAll<T>( string key );

        //Set commands
        bool SAdd<T>( string key, T value );
        IEnumerable<bool> SAdd<T>( IEnumerable<Tuple<string, T>> pairs );
        bool SRem<T>( string key, T value );
        TValue SPop<TValue>( string key );
        bool SMove<T>( string srcKey, string destKey, T value );
        int SCard( string key );
        bool SIsMember<T>( string key, T value );
        IEnumerable<T> SInter<T>( IEnumerable<string> key );
        bool SInterStore<T>( IEnumerable<string> key, string destKey );
        IEnumerable<T> SUnion<T>( IEnumerable<string> key );
        bool SUnionStore<T>( IEnumerable<string> key, string destKey );
        IEnumerable<T> SDiff<T>( IEnumerable<string> key );
        bool SDiffStore<T>( IEnumerable<string> key, string destKey );
        IEnumerable<T> SMembers<T>( string key );
        TValue SRandMember<TValue>( string key );
    }
}