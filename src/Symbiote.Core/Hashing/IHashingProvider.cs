namespace Symbiote.Core.Hashing
{
    public interface IHashingProvider
    {
        long Hash<T>(T value);
    }
}