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

using System;
using System.Runtime.Serialization;
using Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl.ProtoBuf.Request
{
    [Serializable, DataContract( Name = "RpbPutReq" )]
    public class Persist : RiakCommand<Persist, Persisted>
    {
        [DataMember( Order = 1, IsRequired = true, Name = "bucket" )]
        public byte[] Bucket { get; set; }

        [DataMember( Order = 2, IsRequired = true, Name = "key" )]
        public byte[] Key { get; set; }

        [DataMember( Order = 3, IsRequired = false, Name = "vclock" )]
        public byte[] VectorClock { get; set; }

        [DataMember( Order = 4, IsRequired = true, Name = "content" )]
        public RiakContent Content { get; set; }

        [DataMember( Order = 5, IsRequired = false, Name = "w" )]
        public uint Write { get; set; }

        [DataMember( Order = 6, IsRequired = false, Name = "dw" )]
        public uint Dw { get; set; }

        [DataMember( Order = 7, IsRequired = false, Name = "return_body" )]
        public bool ReturnBody { get; set; }

        public Persist() {}

        public Persist(string bucket, string key, RiakContent content, uint write, uint dw, bool returnBody)
        {
            Bucket = bucket.ToBytes();
            Key = key.ToBytes();
            Content = content;
            Write = write.ToggleEndianicity();
            Dw = dw.ToggleEndianicity();
            ReturnBody = returnBody;
        }

        public Persist( string bucket, string key, string vectorClock, RiakContent content, uint write, uint dw, bool returnBody )
            : this (bucket, key, content, write, dw, returnBody)
        {
            // Not sure why, but this causes the Riak server to fail to respond at ALL
            //VectorClock = vectorClock.ToBytes();
        }
    }
}