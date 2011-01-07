using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Symbiote.Riak.Impl.Responses
{
    public class ServerInfo
    {
        public string Node { get; set; }
        public string ServerVersion { get; set; }
    }
}
