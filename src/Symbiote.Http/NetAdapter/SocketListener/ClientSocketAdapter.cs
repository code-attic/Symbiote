using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Symbiote.Core.Extensions;
using Symbiote.Core.Utility;
using Symbiote.Http.Owin;
using Symbiote.Http.Owin.Impl;

namespace Symbiote.Http.NetAdapter.SocketListener
{
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
                    socket.BeginDisconnect( false, OnDisconnect, null );
                    LaunchApplication = null;
                    Remove = null;
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e );
                }
            }
        }

        public void OnDisconnect( IAsyncResult result )
        {
            socket.EndDisconnect( result );
            socket.Close();
        }

        public bool Read()
        {
            WaitForReceive();
            return true;
        }

        public void OnReceive( IAsyncResult result )
        {
            var total = socket.EndReceive( result );
            var buffer = new byte[total];
            if( total > 0 )
            {
                Buffer.BlockCopy( Bytes, 0, buffer, 0, total );
                ConcreteRequest.BytesReceived( buffer );
                LaunchApplication( this );
                LaunchApplication = x => { };
            }
        }

        public void WaitForReceive()
        {
            try
            {
                if ( socket.Connected && !Disposed )
                {
                    Bytes = new byte[BufferSize];
                    SocketError error;
                    socket.BeginReceive( Bytes, 0, BufferSize, SocketFlags.None, out error, OnReceive, null );
                }
            }
            catch
            {
                Close();
            }
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