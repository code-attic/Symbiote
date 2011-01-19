using Symbiote.Core.Impl.Collections;

namespace Symbiote.Riak.Impl
{
    public class VectorRegistry 
        : ITrackVectors
    {
        protected MruDictionary<string, string> Vectors { get; set; }

        public string GetVectorFor( string key )
        {
            string value;
            Vectors.TryGetValue( key, out value );
            return value;
        }

        public void SetVectorFor( string key, string vector )
        {
            Vectors[key] = vector;
        }

        public VectorRegistry()
        {
            Vectors = new MruDictionary<string, string>();
        }
    }
}