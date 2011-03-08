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
    public class ClientSocketAdapter :
        IContext,
        IResponseAdapter,
        IDisposable
    {
        protected Request ConcreteRequest { get; set; }
        public readonly int BufferSize = 8 * 1024;
        public byte[] Bytes { get; set; }
        public string Id { get; set; }
        public bool Disposed { get; set; }
        public Socket Socket { get; set; }
        public IRequest Request { get { return ConcreteRequest; } }
        public IResponseAdapter Response { get { return this; } }
        public Action<IContext> LaunchApplication { get; set; }
        public Action<string> Remove { get; set; }

        public void Close()
        {
            if ( !Disposed )
            {
                Disposed = true;
                Remove( Id );
                Socket.BeginDisconnect( true, OnDisconnect, null );
                LaunchApplication = null;
                Remove = null;
            }
        }

        public void OnDisconnect( IAsyncResult result )
        {
            Socket.EndDisconnect( result );
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

            if( !Disposed )
                WaitForReceive();
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
                Socket.Send( headerBuffer );
                Socket.Send( bodyBuffer );
                Close();
            }
            catch ( SocketException socketException )
            {
                Close();
            }
        }

        public void WaitForReceive()
        {
            try
            {
                if ( Socket.Connected && !Disposed )
                {
                    Bytes = new byte[BufferSize];
                    SocketError error;
                    Socket.BeginReceive( Bytes, 0, BufferSize, SocketFlags.None, out error, OnReceive, null );
                }
            }
            catch
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
            Id = socket.Handle.ToString();
            Socket = socket;
            LaunchApplication = launchApplication;
            ConcreteRequest = new Request();
            Remove = remove;
            WaitForReceive();
        }

        public void Dispose()
        {
            Close();
        }
    }
}