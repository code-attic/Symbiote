namespace Symbiote.Riak.Impl.Data
{
    public class Document<T>
    {
        public string Charset { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public uint LastModified { get; set; }
        public uint LastModifiedInSeconds { get; set; }
        public T Value { get; set; }
        public string VectorClock { get; set; }

        public Document() {}
    }

    public class Link
    {
        public string Bucket { get; set; }
        public string Key { get; set; }
        public string Tag { get; set; }
    }
}