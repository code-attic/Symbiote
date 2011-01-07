using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.ProtoBuf.Data.Response
{
    [Serializable, DataContract(Name = "RpbBucketProps")]
    public class BucketProperties
    {
        [DataMember(Order = 1, IsRequired = false, Name = "n_val")]
        public uint NValue { get; set; }

        [DataMember(Order = 2, IsRequired = false, Name = "allow_mult")]
        public bool AllowMultiple { get; set; }
    }
}