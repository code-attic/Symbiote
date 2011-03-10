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
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Utility;
using Symbiote.Http.Owin;

namespace Symbiote.Http.Impl.Adapter.SocketListener
{
    public class ClientSocketNode
    {
        public string Id { get { return ClientSocket == null ? "" : ClientSocket.Id; } }
        public ClientSocketNode Next { get; set; }
        public ClientSocketNode Previous { get; set; }
        public IClientSocketAdapter ClientSocket { get; set; }
        public Action<string> Remove { get; set; }
        public static readonly object ChangeLock = new object();

        public void Delete( string id )
        {
            lock( ChangeLock )
            {
                Previous.Next = Next;
                Next.Previous = Previous;
                ClientSocket = null;
                if( Remove != null )
                    Remove( id );
            }
        }

        public void Process()
        {
            try
            {
                if( ClientSocket != null && !ClientSocket.Read() )
                {
                    Delete( Id );
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        public ClientSocketNode Add( IClientSocketAdapter adapter )
        {
            lock( ChangeLock )
            {
                return new ClientSocketNode( Previous, this, adapter );
            }
        }

        public ClientSocketNode()
        {
            Next = this;
            Previous = this;
            ClientSocket = null;
        }

        public ClientSocketNode( ClientSocketNode previous, ClientSocketNode next, IClientSocketAdapter clientSocket )
        {
            Next = next;
            Previous = previous;
            Previous.Next = this;
            Next.Previous = this;
            ClientSocket = clientSocket;
            Remove = ClientSocket.Remove;
            ClientSocket.Remove = Delete;
        }

        public override string ToString()
        {
            var builder = new DelimitedBuilder("-");
            var node = this;
            do
            {
                builder.Append( node.Id );
                node = node.Next;
            } while ( node.Id != Id );
            return builder.ToString();
        }
    }

    public interface IClientSocketAdapter
    {
        string Id { get; }
        void Close();
        bool Read();
        Action<string> Remove { get; set; }
    }

    public class ClientSocketAdapter :
        IClientSocketAdapter,
        IContext,
        IResponseAdapter,
        IDisposable
    {
        private object _readLock = new object();
        protected Socket socket;
        protected Request ConcreteRequest { get; set; }
        public readonly int BufferSize = 8 * 1024;
        public byte[] Bytes { get; set; }
        public string Id { get { return socket.Handle.ToString(); } }
        public bool Disposed { get; set; }
        public bool Reading { get; set; }
        public IRequest Request { get { return ConcreteRequest; } }
        public IResponseAdapter Response { get { return this; } }
        public Action<IContext> LaunchApplication { get; set; }
        public Action<string> Remove { get; set; }

        public void Close()
        {
            if ( !Disposed )
            {
                try
                {
                    Disposed = true;
                    Remove( Id );
                    socket.Shutdown( SocketShutdown.Both );
                    socket.Disconnect( false );
                    socket.Close();
                    LaunchApplication = null;
                    Remove = null;
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e );
                }
            }
        }

        public bool Read()
        {
            if(!Reading)
            {
                lock( _readLock )
                {
                    if ( !Reading )
                    {
                        Reading = true;
                        bool valid = false;
                        if( !Disposed && socket != null )
                        {
                            try
                            {
                                var buffer = new byte[BufferSize];
                                var total = socket.Receive( buffer );
                                if ( total > 0 )
                                {
                                    ConcreteRequest.BytesReceived( buffer );
                                    LaunchApplication( this );
                                    LaunchApplication = x => { };
                                }
                                valid = true;
                            }
                            catch ( Exception e )
                            {
                                Console.WriteLine( e );
                            }
                        }
                        Reading = false;
                        return valid;
                    }
                }
            }
            return true;
        }

        public void Respond( string status, IDictionary<string, string> headers, IEnumerable<object> body )
        {
            try
            {
                var builder = new DelimitedBuilder("\r\n");
                var headerBuilder = new HeaderBuilder( headers );
                var responseBody = new MemoryStream();

                body.ForEach( x => ResponseEncoder.Write( x, responseBody ) );

                builder.AppendFormat( "HTTP/1.1 {0}", status );
                headerBuilder.ContentLength( responseBody.Length );
                headerBuilder.Date( DateTime.UtcNow );

                headers.ForEach( x => builder.AppendFormat( "{0}: {1}", x.Key, x.Value ) );
                builder.Append( "\r\n" );
                var header = builder.ToString();
                var headerBuffer = Encoding.UTF8.GetBytes( header );
                var bodyBuffer = responseBody.GetBuffer();
                socket.Send( headerBuffer );
                socket.Send( bodyBuffer );
                Close();
            }
            catch ( SocketException socketException )
            {
                Close();
            }
        }

        public ClientSocketAdapter( 
                             Socket socket,
                             Action<string> remove,
                             Action<IContext> launchApplication
            )
        {
            this.socket = socket;
            LaunchApplication = launchApplication;
            ConcreteRequest = new Request();
            Remove = remove;
        }

        public void Dispose()
        {
            Close();
        }
    }
}