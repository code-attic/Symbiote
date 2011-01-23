/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System.Net.Sockets;

namespace Symbiote.Riak.Impl.ProtoBuf.Connection
{
    public class ProtoBufConnection
        : IProtoBufConnection
    {
        protected readonly int DEFAULT_BUFFER_SIZE = 16 * 1024;
        protected RiakSerializer Serializer { get; set; }
        public NetworkStream Stream { get; protected set; }
        public TcpClient Client { get; protected set; }
        public int SendLength { get; protected set; }
        public RiakNode Node { get; set; }

        public object Send<T>(T command)
        {
            var bytes = Serializer.GetCommandBytes( command );
            Stream.Write(bytes, 0, bytes.Length);
            return Read();
        }

        public object Read()
        {
            var result = Serializer.GetResult(Stream);
            return result;
        }

        public ProtoBufConnection( RiakNode node )
        {
            Node = node;
            Serializer = new RiakSerializer();
            Client = new TcpClient(Node.NodeAddress, Node.ProtoBufPort);
            Stream = Client.GetStream();
            Client.SendBufferSize = DEFAULT_BUFFER_SIZE;
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