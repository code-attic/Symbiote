using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Riak.Config;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public class ProtoBufConnectionProvider
        : IConnectionProvider
    {
        public IRiakConfiguration Configuration { get; set; }

        public IRiakConnection Connection { get; protected set; }

        public IRiakConnection GetConnection()
        {
            Connection = Connection ?? new ProtoBufConnection( Configuration.Nodes.First() );
            return Connection;
        }

        public ProtoBufConnectionProvider( IRiakConfiguration configuration )
        {
            Configuration = configuration;
        }
    }
}
