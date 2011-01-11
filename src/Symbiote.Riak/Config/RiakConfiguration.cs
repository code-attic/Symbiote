using System.Collections.Generic;
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public class RiakConfiguration : IRiakConfiguration
    {
        public List<RiakNode> Nodes { get; set; }

        public RiakConfiguration()
        {
            Nodes = new List<RiakNode>();
        }
    }
}