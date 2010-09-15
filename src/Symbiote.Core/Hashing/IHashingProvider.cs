namespace Symbiote.Core.Hashing
{
    public interface IHashingProvider
    {
        int Hash<T>(T value);
    }
}