using System;
using System.Collections.Generic;

namespace Symbiote.Core.DI
{
    public interface IDependencyRegistry
    {
        IEnumerable<Type> RegisteredPluginTypes { get; }
        Type GetDefaultTypeFor<T>();
        bool HasPluginFor<T>();
        void Register(IDependencyDefinition dependency);
        void Scan(IScanInstruction scanInstruction);
    }
}