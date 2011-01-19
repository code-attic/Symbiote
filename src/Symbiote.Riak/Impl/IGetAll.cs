using System.Collections.Generic;

namespace Symbiote.Riak.Impl
{
    public interface IGetAll
    {
        IEnumerable<T> GetAll<T>();
    }
}