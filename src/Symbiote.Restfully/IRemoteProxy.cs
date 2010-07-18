using System;
using System.Linq.Expressions;

namespace Symbiote.JsonRpc.Client
{
    public interface IRemoteProxy<T> where T : class
    {
        void Call(Expression<Action<T>> call);
        R Call<R>(Expression<Func<T,R>> call);
    }
}