using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Symbiote.Riak.Impl.ProtoBuf
{
    public class ProtoBufConnection
        : IRiakConnection
    {
        protected Stream _stream;
        protected TcpClient _client;
        protected int? _sendLength;
        protected RiakSerializer Serializer { get; set; }
        public Stream Stream { get; protected set; }
        public TcpClient Client { get; protected set; }
        public int SendLength { get; protected set; }
        public RiakNode Node { get; set; }

        //public TcpClient Client
        //{
        //    get
        //    {
        //        _client = _client ??
        //               new TcpClient( Node.NodeAddress, Node.ProtoBufPort );
        //        return _client;
        //    }
        //}

        //public int SendLength
        //{
        //    get
        //    {
        //        _sendLength = _sendLength ?? Client.SendBufferSize;
        //        return _sendLength.Value;
        //    }
        //}

        //public Stream Stream
        //{
        //    get
        //    {
        //        _stream = _stream ??
        //               Client.GetStream();
        //        return _stream;
        //    }
        //}

        public object Send<T>(T command)
        {
            var bytes = Serializer.GetCommandBytes( command );
            Stream.Write( bytes, 0, bytes.Length );
            return Read();
        }

        public object Read()
        {
            return Serializer.GetResult( Stream );
        }

        public ProtoBufConnection( RiakNode node )
        {
            Node = node;
            Serializer = new RiakSerializer();
            Client = new TcpClient(Node.NodeAddress, Node.ProtoBufPort);
            Stream = Client.GetStream();
            SendLength = Client.SendBufferSize;
            Client.ReceiveTimeout = 1000;
        }
    }
}