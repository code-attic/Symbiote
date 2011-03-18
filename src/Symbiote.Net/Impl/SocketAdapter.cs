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
        public Stream SocketStream { get; set; }
        public bool WaitingOnBytes { get; set; }
        public bool WaitingOnWrite { get; set; }
        public Action WriteCompleted { get; set; }
        public SemaphoreSlim ReadLock { get; set; }
        public SemaphoreSlim WriteLock { get; set; }

        public void Close()
        {
            if(!Disposed)
            {
                Disposed = true;
                ReadLock.Wait();
                WriteLock.Wait();
                try
                {
                    if( Connection.Connected )
                    {
                        SocketStream.Flush();
                        Connection.Shutdown( SocketShutdown.Both );
                    }
                    Connection.Close();
                    Bytes = new byte[0];
                }
                finally
                {
                    Connection.Close();
                    Connection = null;
                    ReadLock.Dispose();
                    WriteLock.Dispose();
                }
            }
        }

        public void OnRead( IAsyncResult result )
        {
            WaitingOnBytes = false;
            var read = SocketStream.EndRead( result );
            ReadLock.Release();
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
            if( !WaitingOnBytes && Connection.Available > 0 )
            {
                ReadLock.Wait();
                try
                {
                    OnBytes = onBytes;
                    SocketStream.BeginRead( Bytes, 0, Configuration.BufferSize, OnRead, null );
                    return true;
                }
                catch( Exception ex )
                {
                    ReadLock.Release();
                }
            }
            return false;
        }

        public bool Write( ArraySegment<byte> bytes, Action onComplete )
        {
            if( !WaitingOnWrite )
            {
                WriteLock.Wait();
                try
                {
                    WriteCompleted = onComplete;
                    SocketStream.BeginWrite( bytes.Array, bytes.Offset, bytes.Count, OnWrite, null );
                }
                catch ( IOException ioex )
                {
                    WriteLock.Release();
                }
                catch ( Exception ex )
                {
                    WriteLock.Release();
                }
            }
            return false;
        }

        public SocketAdapter( Socket connection, SocketConfiguration configuration )
        {
            Configuration = configuration;
            Connection = connection;
            Bytes = new byte[configuration.BufferSize];
            SocketStream = new NetworkStream( connection );
            ReadLock = new SemaphoreSlim( 1 );
            WriteLock = new SemaphoreSlim( 1 );
            //Id = Guid.NewGuid().ToString();
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