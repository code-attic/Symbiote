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

namespace Symbiote.Http.Impl.Adapter.SocketListener
{
    public class ClientSocket
        : IDisposable
    {
        public string Id { get; set; }
        public Socket Socket { get; set; }
        public ClientSocket Next { get; set; }
        public ClientSocket Previous { get; set; }

        public Action OnError { get; set; }
        public Action<ArraySegment<byte>> OnData { get; set; }
        public Action Remove { get; set; }

        public byte[] Bytes { get; set; }
        public readonly int BufferSize = 8 * 1024;

        public void Disconnect()
        {
            Socket.Close();
            Socket = null;
            if( !Next.Id.Equals( Id ) )
                Previous.Next = Next;
            if( !Previous.Id.Equals( Id ) )
                Next.Previous = Previous;
            Remove();
        }

        public ClientSocket Add( string id, 
                                 Socket socket,
                                 Action remove, 
                                 Action<ArraySegment<byte>> onData,
                                 Action onError )
        {
            var newNode = new ClientSocket( id, socket, Next, this, onError, onData, remove );
            Next.Previous = newNode;
            Next = newNode;
            return newNode;
        }

        public void WaitForReceive()
        {
            if( !Socket.Connected )
                Disconnect();
            Bytes = new byte[BufferSize];
            Socket.BeginReceive( Bytes, 0, BufferSize, SocketFlags.OutOfBand, OnReceive, null );
        }

        public void OnReceive( IAsyncResult result )
        {
            var total = Socket.EndReceive( result );
            var buffer = new byte[total];
            if( total > 0 )
            {
                Buffer.BlockCopy( Bytes, 0, buffer, 0, total );
                OnData( new ArraySegment<byte>( Bytes, 0, total ) );
            }
            WaitForReceive();
        }

        public ClientSocket( string id, 
                             Socket socket, 
                             ClientSocket next, 
                             ClientSocket previous, 
                             Action onError, 
                             Action<ArraySegment<byte>> onData, 
                             Action remove )
        {
            Id = id;
            Socket = socket;
            Next = next;
            Previous = previous;
            OnError = onError;
            OnData = onData;
            Remove = remove;

            WaitForReceive();
        }

        public void Dispose()
        {
            Socket.Dispose();
        }
    }
}