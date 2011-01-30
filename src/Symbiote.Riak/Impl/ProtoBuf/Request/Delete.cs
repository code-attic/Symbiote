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
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbDelReq" )]
    public class Delete : RiakCommand<Delete, Deleted>
    {
        [DataMember( Order = 1, IsRequired = true, Name = "bucket" )]
        public byte[] Bucket { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "key" )]
        public byte[] Key { get; set; }

        [DataMember( Order = 3, IsRequired = false, Name = "rw" )]
        public uint SuccessfulDeletionCount { get; set; }

        public Delete()
        {
        }

        public Delete( string bucket, string key, uint minimum )
        {
            Bucket = bucket.ToBytes();
            Key = key.ToBytes();
            SuccessfulDeletionCount = minimum.ToggleEndianicity();
        }
    }
}