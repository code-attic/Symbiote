using System;

namespace Core.Tests.DI.Container
{
    public class OpenImpl<T> : ITakeGenericParams<T>
    {
        public Type GetTypeOfT
        {
            get { return typeof( T ); }
        }
    }
}