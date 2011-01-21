using System;
using System.IO;
using System.Net.Sockets;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class ProtoBufConnection
        : IProtoBufConnection, IDisposable
    {
        protected Stream _stream;
        protected TcpClient _client;
        protected int? _sendLength;
        protected RiakSerializer Serializer { get; set; }
        public NetworkStream Stream { get; protected set; }
        public TcpClient Client { get; protected set; }
        public int SendLength { get; protected set; }
        public RiakNode Node { get; set; }

        public object Send<T>(T command)
        {
            var bytes = Serializer.GetCommandBytes( command );
            Stream.Write( bytes, 0, bytes.Length );
            Stream.Flush();
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
        }

        public void Dispose()
        {
            if(Stream != null)
                Stream.Close();
            
            if(Client != null && Client.Connected)
                Client.Close();
        }
    }
}