using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Symbiote.Core;

namespace Symbiote.Riak
{
    public static class RiakAssimilation
    {
        public static IAssimilate Riak(this IAssimilate assimilate, Action<RiakConfigurator> configurate)
        {
            var configurator = new RiakConfigurator();
            configurate( configurator );

            Assimilate.Dependencies( x =>
            {
                x.For<IRiakConfiguration>().Use( configurator.Configuration );
            });

            return assimilate;
        }
    }

    public class RiakNode
    {
        public string NodeAddress { get; set; }
        public int ProtoBufPort { get; set; }
        public int HttpPort { get; set; }

        public RiakNode Address(string address)
        {
            NodeAddress = address;
            return this;
        }

        public RiakNode ForHttp(int httpPort)
        {
            HttpPort = httpPort;
            return this;
        }

        public RiakNode ForProtocolBufferPort(int protocolBufferPort)
        {
            ProtoBufPort = protocolBufferPort;
            return this;
        }

        public RiakNode()
        {
            NodeAddress = "127.0.0.1";
            ProtoBufPort = 8087;
            HttpPort = 8091;
        }
    }

    public interface IRiakConfiguration {
        List<RiakNode> Nodes { get; set; }
    }

    public class RiakConfiguration : IRiakConfiguration
    {
        public List<RiakNode> Nodes { get; set; }
    }

    public class RiakConfigurator
    {
        public RiakConfiguration Configuration { get; protected set; }

        public RiakConfigurator AddLocalNode()
        {
            Configuration.Nodes.Add( new RiakNode() );
            return this;
        }

        public RiakConfigurator AddNode(Action<RiakNode> riakNodeConfiguration)
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
