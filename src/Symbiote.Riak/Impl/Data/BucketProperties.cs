namespace Symbiote.Riak.Impl.Data
{
    public class BucketProperties
    {
        public uint NValue { get; set; }
        public bool AllowMultiple { get; set; }

        public BucketProperties( uint nValue, bool allowMultiple )
        {
            NValue = nValue;
            AllowMultiple = allowMultiple;
        }
    }
}