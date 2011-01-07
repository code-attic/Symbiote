using System;
using Symbiote.Riak.Impl;

namespace Symbiote.Riak.Config
{
    public class RiakConfigurator
    {
        public RiakConfiguration Configuration { get; protected set; }

        public RiakConfigurator AddLocalNode()
        {
            Configuration.Nodes.Add( new RiakNode() );
            return this;
        }

        public RiakConfigurator AddNode( Action<RiakNode> riakNodeConfiguration )
        {
            var node = new RiakNode();
            riakNodeConfiguration( node );
            Configuration.Nodes.Add( node );
            return this;
        }

        public RiakConfigurator()
        {
            Configuration = new RiakConfiguration();
        }
    }
}