﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Request
{
    [Serializable, DataContract(Name = "RpbMapRedReq")]
    public class RunMapReduce
    {
        [DataMember(Order = 1, IsRequired = true, Name = "request")]
        public byte[] Request { get; set; }

        [DataMember(Order = 2, IsRequired = true, Name = "content_type")]
        public byte[] ContentType { get; set; }

        public RunMapReduce() {}

        public RunMapReduce( string request, string contentType )
        {
            Request = request.ToBytes();
            ContentType = contentType.ToBytes();
        }
    }
}