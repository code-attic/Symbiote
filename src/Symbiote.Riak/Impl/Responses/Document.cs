namespace Symbiote.Riak.Impl.Responses
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
}