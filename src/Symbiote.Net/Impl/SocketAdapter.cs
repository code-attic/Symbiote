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
		public IAsyncResult WriteHandle { get; set; }
        public Stream SocketStream { get; set; }
        public bool WaitingOnBytes { get; set; }
        public bool WaitingOnWrite { get; set; }
        public Action WriteCompleted { get; set; }
		//public SemaphoreSlim WriteLock { get; set; }
		public static long Total;

        public void Close()
        {
            if( !Disposed )
            {
                Disposed = true;
                try
                {
					if( WriteHandle != null && WriteHandle.AsyncWaitHandle != null && !WriteHandle.IsCompleted )
						WriteHandle.AsyncWaitHandle.WaitOne();
					
                    if( SocketStream != null )
                        SocketStream.Close();

                    if( Connection != null )
                        Connection.Close();

                    if( ReadHandle != null && ReadHandle.AsyncWaitHandle != null )
                        ReadHandle.AsyncWaitHandle.Dispose();
					
                    Bytes = new byte[0];
                }
                finally
                {
                    SocketStream = null;
                    Connection = null;
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
			try
			{
				if( !Disposed )
				{
					SocketStream.EndWrite( result );
					WriteCompleted();
				}
				else if(WriteHandle != null && WriteHandle.AsyncWaitHandle != null )
				{
					WriteHandle.AsyncWaitHandle.Dispose();
				}
	            	
			}
            catch( Exception ex )
			{
				if(WriteHandle != null && WriteHandle.AsyncWaitHandle != null )
					WriteHandle.AsyncWaitHandle.Dispose();
				Console.WriteLine( ex );
			}
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
					Console.WriteLine( ex );
                }
            }
            return false;
        }

        public bool Write( ArraySegment<byte> bytes, Action onComplete )
        {
            try
            {
                //WriteLock.Wait();
				WriteCompleted = onComplete;				
                WriteHandle = SocketStream.BeginWrite( bytes.Array, bytes.Offset, bytes.Count, OnWrite, null );
                return true;
            }
            catch ( IOException ioex )
            {
				//WriteLock.Release();
				Console.WriteLine( ioex );
            }
            catch ( Exception ex )
            {
				//WriteLock.Release();
				Console.WriteLine( ex );
            }
            return false;
        }

        public SocketAdapter( Socket connection, SocketConfiguration configuration )
        {
            try 
			{
				Console.WriteLine( "Created {1}: {0}", Total ++, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
				Configuration = configuration;
				Connection = connection;
				Bytes = new byte[configuration.BufferSize];
				SocketStream = new NetworkStream( connection );
				//WriteLock = new SemaphoreSlim( 1 );
			} 
			catch (Exception ex) 
			{
				Console.WriteLine( ex );	
			}
            //Id = Guid.NewGuid().ToString();
        }

        public void Dispose()
        {
            if( !Disposed )
            {
                Close();
            }
        }
		
		~SocketAdapter()
		{
			Console.WriteLine( "Remaining {1}: {0}", --Total, DateTime.UtcNow.TimeOfDay.TotalMilliseconds );
		}
    }
}