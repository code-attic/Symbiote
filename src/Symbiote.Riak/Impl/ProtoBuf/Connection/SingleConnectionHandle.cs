using System;
using System.Linq;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class SingleConnectionHandle
        : IConnectionHandle
    {
        protected IConnectionFactory Factory { get; set; }
        public IProtoBufConnection Connection { get; set; }

        public SingleConnectionHandle( IProtoBufConnection connection )
        {
            Connection = connection;
        }

        public void Dispose()
        {
            Connection.Dispose();
            Connection = null;
        }
    }
}
