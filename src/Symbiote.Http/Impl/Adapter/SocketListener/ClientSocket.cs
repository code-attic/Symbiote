// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Net.Sockets;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.SocketListener
{
    public class ClientSocketAdapter :
        IContext,
        IDisposable
    {
        protected Request ConcreteRequest { get; set; }
        public readonly int BufferSize = 8 * 1024;
        public byte[] Bytes { get; set; }
        public string Id { get; set; }
        public Socket Socket { get; set; }
        public ClientSocketAdapter Next { get; set; }
        public ClientSocketAdapter Previous { get; set; }
        public IRequest Request { get { return ConcreteRequest; } }
        public IResponseAdapter Response { get; set; }
        public Action<IContext> LaunchApplication { get; set; }

        public void Disconnect()
        {
            if(Socket != null && Socket.Connected)
            {
                Socket.Disconnect( true );
                Socket.Close();
                Socket.Dispose();
            }
            Previous.Next = Next;
            Next.Previous = Previous;
            Next = null;
            Previous = null;
        }

        public ClientSocketAdapter Add( string id, Socket socket, Action<IContext> launchApplication )
        {
            var newNode = new ClientSocketAdapter( id, socket, Next, this, launchApplication );
            Next.Previous = newNode;
            Next = newNode;
            return newNode;
        }

        public void WaitForReceive()
        {
            if( !Socket.Connected )
            {
                Disconnect();
                return;
            }
            Bytes = new byte[BufferSize];
            try
            {
                Socket.BeginReceive( Bytes, 0, BufferSize, SocketFlags.None, OnReceive, null );
            }
            catch ( Exception e )
            {
                Disconnect();
            }
        }

        public void OnReceive( IAsyncResult result )
        {
            var total = Socket.EndReceive( result );
            var buffer = new byte[total];
            if( total > 0 )
            {
                Buffer.BlockCopy( Bytes, 0, buffer, 0, total );
                ConcreteRequest.BytesReceived( buffer );
                LaunchApplication( this );
                LaunchApplication = x => { };
            }
            WaitForReceive();
        }

        public ClientSocketAdapter()
        {
            Next = this;
            Previous = this;
        }

        public ClientSocketAdapter( string id, 
                             Socket socket,
                             ClientSocketAdapter next, 
                             ClientSocketAdapter previous,
                             Action<IContext> launchApplication
            )
        {
            Id = id;
            Socket = socket;
            LaunchApplication = launchApplication;
            ConcreteRequest = new Request();
            Response = new SocketResponseAdapter( socket );
            Next = next ?? this;
            Previous = previous ?? this;
            WaitForReceive();
        }

        public void Dispose()
        {
            if( Socket != null )
                Socket.Dispose();
            LaunchApplication = null;
        }
    }
}