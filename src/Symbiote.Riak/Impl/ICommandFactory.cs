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
using Symbiote.Riak.Impl.Data;
using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl
{
    public interface ICommandFactory
    {
        Delete CreateDelete( string bucket, string key, uint minimum );
        Get CreateGet( string bucket, string key, uint minimum );
        GetBucketProperties CreateGetBucketProperties( string bucket );
        GetClientId CreateGetClientId();
        GetServerInfo CreateGetServerInfo();
        ListBuckets CreateListBuckets();
        ListKeys CreateListKeys( string bucket );

        Persist CreatePersistExisting( string bucket, string key, string vectorClock, RiakContent content, uint write,
                                       uint dw, bool returnBody );

        Persist CreatePersistNew( string bucket, string key, RiakContent content, uint write, uint dw, bool returnBody );
        Ping CreatePing();
        RunMapReduce CreateMapReduce( string mapReduce, string contentType );
        SetBucketProperties CreateSetBucketProperties( string bucket, BucketProperties properties );
        SetClientId CreateSetClientId( string clientId );
    }
}