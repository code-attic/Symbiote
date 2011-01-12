using System.Linq;
using Symbiote.Riak.Config;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class ConnectionFactory
        : IConnectionFactory
    {
        public IRiakConfiguration Configuration { get; set; }

        public IProtoBufConnection GetConnection()
        {
            return new ProtoBufConnection( Configuration.Nodes.First() );
        }

        public ConnectionFactory( IRiakConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}