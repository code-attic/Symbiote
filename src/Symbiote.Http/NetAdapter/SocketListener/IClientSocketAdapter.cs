using System;

namespace Symbiote.Http.NetAdapter.SocketListener
{
    public interface IClientSocketAdapter
    {
        string Id { get; }
        void Close();
        bool Read();
        Action<string> Remove { get; set; }
    }
}