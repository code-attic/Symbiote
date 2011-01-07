namespace Symbiote.Riak.Impl.Data
{
    public class ServerInfo
    {
        public string Node { get; set; }
        public string ServerVersion { get; set; }

        public ServerInfo() {}

        public ServerInfo( string node, string serverVersion )
        {
            Node = node;
            ServerVersion = serverVersion;
        }
    }
}