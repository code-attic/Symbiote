using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Symbiote.Net
{
    public class SocketAdapter
        : ISocketAdapter
    {
        public byte[] Bytes { get; set; }
        public SocketConfiguration Configuration { get; set; }
        public Socket Connection { get; set; }
        public bool Disposed { get; set; }    
        public string Id { get; protected set; }
        public Action<ArraySegment<byte>> OnBytes { get; set; }
        public IAsyncResult ReadHandle { get; set; }
        public Stream SocketStream { get; set; }
        public bool WaitingOnBytes { get; set; }
        public bool WaitingOnWrite { get; set; }
        public Action WriteCompleted { get; set; }
        public SemaphoreSlim WriteLock { get; set; }

        public void Close()
        {
            if(!Disposed)
            {
                WriteLock.Wait();
                Disposed = true;
                try
                {
                    if( SocketStream != null )
                        SocketStream.Close();

                    if( Connection != null )
                        Connection.Close();

                    if( ReadHandle != null )
                        ReadHandle.AsyncWaitHandle.Dispose();

                    Bytes = new byte[0];
                }
                finally
                {
                    SocketStream = null;
                    Connection = null;
                    WriteLock.Release();
                    WriteLock.Dispose();
                }
            }
        }

        public void OnRead( IAsyncResult result )
        {
            WaitingOnBytes = false;
            var read = SocketStream.EndRead( result );
            OnBytes( new ArraySegment<byte>( Bytes, 0, read ));
        }

        public void OnWrite( IAsyncResult result )
        {
            WaitingOnWrite = false;
            SocketStream.EndWrite( result );
            WriteLock.Release();
            WriteCompleted();
        }

        public bool Read( Action<ArraySegment<byte>> onBytes )
        {
            if( !WaitingOnBytes )
            {
                WaitingOnBytes = true;
                try
                {
                    OnBytes = onBytes;
                    ReadHandle = SocketStream.BeginRead( Bytes, 0, Configuration.BufferSize, OnRead, null );
                    return true;
                }
                catch( Exception ex )
                {
                }
            }
            return false;
        }

        public bool Write( ArraySegment<byte> bytes, Action onComplete )
        {
            WriteLock.Wait();
            try
            {
                WriteCompleted = onComplete;
                SocketStream.BeginWrite( bytes.Array, bytes.Offset, bytes.Count, OnWrite, null );
                return true;
            }
            catch ( IOException ioex )
            {
                WriteLock.Release();
            }
            catch ( Exception ex )
            {
                WriteLock.Release();
            }
            return false;
        }

        public SocketAdapter( Socket connection, SocketConfiguration configuration )
        {
            Configuration = configuration;
            Connection = connection;
            Bytes = new byte[configuration.BufferSize];
            SocketStream = new NetworkStream( connection );
            //Id = Guid.NewGuid().ToString();
            WriteLock = new SemaphoreSlim( 1 );
        }

        public void Dispose()
        {
            if( !Disposed )
            {
                Close();
            }
        }
    }
}