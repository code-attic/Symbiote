using System;

namespace Core.Tests.DI.Container
{
    public interface ITakeGenericParams<T>
    {
        Type GetTypeOfT { get; }
    }
}