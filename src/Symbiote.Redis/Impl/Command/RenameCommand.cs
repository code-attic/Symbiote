using Symbiote.Core.Extensions;
using Symbiote.Redis.Impl.Connection;

namespace Symbiote.Redis.Impl.Command
{
    public class RenameCommand
        : RedisCommand<bool>
    {
        protected const string RENAME_KEY = "RENAME {0} {1}\r\n";
        public string OriginalKey { get; set; }
        public string NewKey { get; set; }

        public bool Rename(IRedisConnection connection)
        {
            return connection.SendExpectString(RENAME_KEY.AsFormat(OriginalKey, NewKey))[0] == '+';
        }

        public RenameCommand(string originalKey, string newKey)
        {
            OriginalKey = originalKey;
            NewKey = newKey;
            Command = Rename;
        }
    }
}