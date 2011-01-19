using System;
using System.Text;
using Symbiote.Core.Impl.Futures;
using Symbiote.Riak.Impl.ProtoBuf.Connection;
using Response = Symbiote.Riak.Impl.ProtoBuf.Response;

namespace Symbiote.Riak.Impl
{
    public interface IRepository :
        IDeleteByKey, IGetByKey, IGetAll
    {
        void Delete<T>( T instance );
        void Persist<T>( T instance );
    }
}
