using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public class ProtoBufConnection
    {
        protected Stream _stream;
        protected TcpClient _client;
        public RiakNode Node { get; set; }
        
        public TcpClient Client
        {
            get
            {
                return _client ??
                       new TcpClient( Node.NodeAddress, Node.ProtoBufPort );
            }
        }

        public Stream Stream 
        { 
            get
            {
                return _stream ??
                       Client.GetStream();
            }
        }

        public ProtoBufConnection( RiakNode node )
        {
            Node = node;
        }
    }
}
