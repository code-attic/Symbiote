using System;
using System.Collections.Generic;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public interface IConnectionFactory
    {
        IProtoBufConnection GetConnection();
    }
}
