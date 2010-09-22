namespace Symbiote.Redis.Impl.Connection
{
    public interface IRedisConnection
    {
        bool InUse { get; set; }
        bool SendExpectSuccess(byte[] data, string command);
        int SendDataExpectInt(byte[] data, string command);
        byte[] SendExpectData(byte[] data, string command);
        string SendExpectString(string command);
        void Connect();
    }
}