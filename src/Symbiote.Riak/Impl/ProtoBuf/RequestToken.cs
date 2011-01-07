using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public enum RequestToken
    {
        Ping = 1,
        GetClientId = 3,
        SetClientId = 5,
        ServerInfo = 7,
        Get = 9,
        Persist = 11,
        Delete = 13,
        ListBuckets = 15,
        ListKeys = 17
    }
}
