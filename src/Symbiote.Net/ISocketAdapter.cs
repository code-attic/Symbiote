using System;

namespace Symbiote.Net
{
    public interface ISocketAdapter
        : IDisposable
    {
        bool Disposed { get; set; }
        string Id { get; }
        bool Read( Action<ArraySegment<byte>> onBytes );
        bool Write( ArraySegment<byte> bytes, Action onComplete );
        void Close();
    }
}