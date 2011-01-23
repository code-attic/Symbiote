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
using Symbiote.Riak.Impl.Data;

namespace Symbiote.Riak
{
    public interface IRiakClient
    {
        bool Delete<T>( string bucket, string key, uint minimumDeletes );
        BucketProperties GetBucketProperties( string bucket );
        Document<T> Get<T>( string bucket, string key, uint reads );
        string GetClientId();
        ServerInfo GetServerInfo();
        IEnumerable<string> GetBucketsList();
        IEnumerable<string> GetKeyList( string bucket );
        bool Ping();
        bool Persist<T>( string bucket, string key, string vectorClock, Document<T> document, uint writeQuorum, uint minimumWrites );
        bool SetBucketProperties( string bucket, BucketProperties properties );
        bool SetClientId( string clientId );
    }
}