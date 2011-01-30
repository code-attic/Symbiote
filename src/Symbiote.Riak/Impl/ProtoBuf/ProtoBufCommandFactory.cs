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
using Symbiote.Riak.Impl.ProtoBuf.Request;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public class ProtoBufCommandFactory : ICommandFactory
    {
        public Delete CreateDelete( string bucket, string key, uint minimum )
        {
            return new Delete( bucket, key, minimum );
        }

        public Get CreateGet( string bucket, string key, uint minimum )
        {
            return new Get( bucket, key, minimum );
        }

        public GetBucketProperties CreateGetBucketProperties( string bucket )
        {
            return new GetBucketProperties( bucket );
        }

        public GetClientId CreateGetClientId()
        {
            return new GetClientId();
        }

        public GetServerInfo CreateGetServerInfo()
        {
            return new GetServerInfo();
        }

        public ListBuckets CreateListBuckets()
        {
            return new ListBuckets();
        }

        public ListKeys CreateListKeys( string bucket )
        {
            return new ListKeys( bucket );
        }

        public Persist CreatePersistNew( string bucket, string key, RiakContent content, uint write, uint dw,
                                         bool returnBody )
        {
            return new Persist( bucket, key, content, write, dw, returnBody );
        }

        public Persist CreatePersistExisting( string bucket, string key, string vectorClock, RiakContent content,
                                              uint write, uint dw, bool returnBody )
        {
            return new Persist( bucket, key, vectorClock, content, write, dw, returnBody );
        }

        public Ping CreatePing()
        {
            return new Ping();
        }

        public RunMapReduce CreateMapReduce( string mapReduce, string contentType )
        {
            return new RunMapReduce( mapReduce, contentType );
        }

        public SetBucketProperties CreateSetBucketProperties( string bucket, Data.BucketProperties properties )
        {
            return new SetBucketProperties( bucket, properties.ToProtoBuf() );
        }

        public SetClientId CreateSetClientId( string clientId )
        {
            return new SetClientId( clientId );
        }
    }
}