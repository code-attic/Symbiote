using System;

namespace Core.Tests.DI.Container
{
    public class ClosedImpl : ITakeGenericParams<string>
    {
        public Type GetTypeOfT
        {
            get { return typeof( string ); }
        }
    }
}