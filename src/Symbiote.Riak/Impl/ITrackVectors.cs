namespace Symbiote.Riak.Impl
{
    public interface ITrackVectors
    {
        string GetVectorFor( string key );
        void SetVectorFor( string key, string vector );
    }
}