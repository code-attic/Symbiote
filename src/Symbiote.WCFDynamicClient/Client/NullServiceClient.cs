using System;
using System.ServiceModel.Description;

namespace Symbiote.Wcf.Client
{
    public class NullServiceClient<T> : IService<T>
        where T : class
    {
        public void Dispose()
        {

        }

        public ClientCredentials ClientCredentials
        {
            get { throw new NotImplementedException(); }
        }

        public void Call(Action<T> call)
        {
            throw new NotImplementedException();
        }

        public TResult Call<TResult>(Func<T, TResult> call)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}