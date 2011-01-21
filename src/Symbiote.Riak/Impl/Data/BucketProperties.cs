using System;
using System.Runtime.Serialization;

namespace Symbiote.Riak.Impl.Data
{
    public class BucketProperties
    {
        public uint NValue { get; set; }
        public bool AllowMultiple { get; set; }

        public ProtoBuf.BucketProperties ToProtoBuf()
        {
            return new ProtoBuf.BucketProperties()
            {
                AllowMultiple = AllowMultiple,
                NValue = NValue
            };
        }

        public BucketProperties( uint nValue, bool allowMultiple )
        {
            NValue = nValue;
            AllowMultiple = allowMultiple;
        }
    }
}