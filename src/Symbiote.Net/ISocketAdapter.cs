using System;

namespace Symbiote.Net
{
    public interface ISocketAdapter
        : IDisposable
    {
		void Check();
        void Close();
		bool Read( Action<ArraySegment<byte>> onBytes, Action<Exception> onException );
        bool Write( ArraySegment<byte> bytes, Action onComplete, Action<Exception> onException );        
    }
}