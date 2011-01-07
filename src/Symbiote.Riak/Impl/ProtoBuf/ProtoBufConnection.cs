using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public RiakNode Node { get; set; }

        public TcpClient Client
        {
            get
            {
                _client = _client ??
                       new TcpClient( Node.NodeAddress, Node.ProtoBufPort );
                return _client;
            }
        }

        public int SendLength
        {
            get
            {
                _sendLength = _sendLength ?? Client.SendBufferSize;
                return _sendLength.Value;
            }
        }

        public Stream Stream
        {
            get
            {
                _stream = _stream ??
                       Client.GetStream();
                return _stream;
            }
        }

        public object Send<T>(T command)
        {
            var bytes = Serializer.GetCommandBytes( command );
            Write(bytes, 0, SendLength > bytes.Length ? SendLength : bytes.Length);
            return Read();
        }

        protected void Write(byte[] bytes, int offset, int count)
        {
            Stream.Write( bytes, offset, count );
            offset += count;
            if(offset < bytes.Length)
                Write(bytes, offset, SendLength > bytes.Length - offset ? SendLength : bytes.Length - offset);
        }

        public object Read()
        {
            return Serializer.GetResult( Stream );
        }

        public ProtoBufConnection( RiakNode node )
        {
            Node = node;
            Serializer = new RiakSerializer();
        }
    }
}