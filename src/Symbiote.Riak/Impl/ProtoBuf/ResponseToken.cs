namespace Symbiote.Riak.Impl.ProtoBuf
{
    public enum ResponseToken
    {
        Error = 0,
        Ping = 2,
        GetClientId = 4,
        SetClientId = 6,
        ServerInfo = 8,
        Get = 10,
        Save = 12,
        Delete = 14,
        ListBuckets = 16,
        ListKeys = 18
    }
}