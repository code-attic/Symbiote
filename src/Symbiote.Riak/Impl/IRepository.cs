using System;
using System.Text;
using Symbiote.Core.Impl.Futures;
using Symbiote.Riak.Config;

namespace Symbiote.Riak.Impl
{
    public interface IRepository
    {
        void Delete(string bucket, string key, uint minimum);
        T Get<T>(string bucket, string key, uint minimum);
        void Persist<T>(string bucket, string key, string vectorClock, T content, uint write, uint dw, bool returnBody);
    }
}
