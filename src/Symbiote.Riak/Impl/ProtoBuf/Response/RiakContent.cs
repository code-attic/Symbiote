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
using System.Runtime.Serialization;
using Symbiote.Core.Serialization;
using Symbiote.Riak.Impl.Data;

namespace Symbiote.Riak.Impl.ProtoBuf.Response
{
    [Serializable, DataContract( Name = "RbpContent" )]
    public class RiakContent
    {
        [DataMember( Order = 1, Name = "value" )]
        public byte[] Value { get; set; }

        [DataMember( Order = 2, IsRequired = false, Name = "content_type" )]
        public byte[] ContentType { get; set; }

        [DataMember( Order = 3, IsRequired = false, Name = "charset" )]
        public byte[] Charset { get; set; }

        [DataMember( Order = 4, IsRequired = false, Name = "content_encoding" )]
        public byte[] ContentEncoding { get; set; }

        [DataMember( Order = 5, IsRequired = false, Name = "vtag" )]
        public byte[] VectorClock { get; set; }

        [DataMember( Order = 6, Name = "links" )]
        public List<RiakLink> Links { get; set; }

        [DataMember( Order = 7, IsRequired = false, Name = "last_mod" )]
        public uint LastMod { get; set; }

        [DataMember( Order = 8, IsRequired = false, Name = "last_mod_usecs" )]
        public uint LastModSecs { get; set; }

        [DataMember( Order = 9, Name = "usermeta" )]
        public List<RiakPair> UserMetadata { get; set; }

        public Document<T> ToDocument<T>()
        {
            return new Document<T>
                       {
                           Charset = Charset.FromBytes(),
                           ContentEncoding = ContentEncoding.FromBytes(),
                           ContentType = ContentType.FromBytes(),
                           LastModified = LastMod,
                           LastModifiedInSeconds = LastModSecs,
                           Value = Value.FromProtocolBuffer<T>(),
                           VectorClock = VectorClock.FromBytes()
                       };
        }

        public RiakContent()
        {
        }

        public RiakContent( byte[] value, string contentType, string charset, string contentEncoding, string vectorClock,
                            uint lastMod, uint lastModSecs )
        {
            Value = value;
            ContentType = contentType.ToBytes();
            Charset = charset.ToBytes();
            ContentEncoding = contentEncoding.ToBytes();
            VectorClock = vectorClock.ToBytes();
            LastMod = lastMod.ToggleEndianicity();
            LastModSecs = lastModSecs.ToggleEndianicity();
        }
    }
}